using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.Contacts.Locals;
using Timer = System.Timers.Timer;

namespace ContactPoint.Contacts.Updater
{
    internal class UpdateWatcher : IDisposable
    {
        public static int MAX_PARALLEL_TASKS_COUNT = 5;

        private readonly ContactsManager _contactsManager;
        private readonly Timer _timer;
        private readonly UpdateTask[] _tasks = new UpdateTask[MAX_PARALLEL_TASKS_COUNT];

        public UpdateWatcher(ContactsManager contactsManager, ISynchronizeInvoke syncInvoke)
        {
            _contactsManager = contactsManager;
            _timer = new Timer(10 * 1000) {SynchronizingObject = syncInvoke, AutoReset = true};
            _timer.Elapsed += TimerElapsed;
            _timer.Start();

            Logger.LogNotice("Starting update contacts watcher.");
        }

        internal void QueueCheck(AddressBookLocal addressBook)
        {
            if (CheckCriteria(addressBook))
            {
                TryUpdateAddressBook(addressBook);
            }
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_contactsManager.AddressBooks.Any())
            {
                Check();
            }
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        private void Check()
        {
            var addressBooks = _contactsManager.AddressBooks.Where(CheckCriteria).ToArray();
            Logger.LogNotice(string.Format("Total {0} address books needs to be updated.", addressBooks.Length));
            foreach (var item in addressBooks)
                TryUpdateAddressBook(item);

            using (new ResourceCriticalOperation((object)_tasks))
            {
                foreach (var task in _tasks)
                    if (task != null && DateTime.Now - task.StartTime > TimeSpan.FromSeconds(5) && !task.IsNotifyingUser)
                    {
                        task.IsNotifyingUser = true;
                        NotifyManager.NotifyUser(new AddressBookUpdatingNotifyControl(task), 0);
                    }
            }
        }

        private UpdateTask CreateTask(AddressBookLocal addressBook)
        {
            Logger.LogNotice(string.Format("Trying to update address book '{0}'.Last update: {1}.", addressBook.Name, addressBook.LastUpdate));
            using (new ResourceCriticalOperation((object)_tasks))
            {
                for (int i = 0; i < MAX_PARALLEL_TASKS_COUNT; i++)
                    if (_tasks[i] == null)
                    {
                        var task = new UpdateTask(_contactsManager, _contactsManager.SqlConnection, addressBook, i);
                        _tasks[i] = task;

                        Logger.LogNotice(string.Format("Found empty slot to perform update for '{0}'.", addressBook.Name));

                        return task;
                    }
            }

            return null;
        }

        private bool CheckCriteria(AddressBookLocal addressBook)
        {
            return 
                addressBook.IsOnline && 
#if DEBUG
                addressBook.LastUpdate < DateTime.Now - TimeSpan.FromSeconds(10) &&
#else
                addressBook.LastUpdate < DateTime.Now - TimeSpan.FromMinutes(10) && 
#endif
                !_tasks.Any(x => x != null && x.AddressBook == addressBook);
        }

        private void TryUpdateAddressBook(AddressBookLocal addressBook)
        {
            UpdateTask updateTask = null;
            using (new ResourceCriticalOperation(addressBook))
                updateTask = CreateTask(addressBook);

            if (updateTask != null)
            {
                var worker = new Task(StartUpdateTask, updateTask, TaskCreationOptions.PreferFairness);
                worker.Start();
            }
        }

        private void StartUpdateTask(object obj)
        {
            var updateTask = obj as UpdateTask;
            if (updateTask != null)
            {
                bool result = false;
                try
                {
                    using (new ResourceCriticalOperation(updateTask.AddressBook))
                    {
                        Logger.LogNotice(String.Format("Updating metadata information for '{0}'.",
                                                       updateTask.AddressBook.Name));

                        updateTask.AddressBook.LastUpdate = DateTime.Now;
                        updateTask.AddressBook.Submit();

                        result = updateTask.Execute();
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e,
                                   string.Format("Problems occured during loading contacts for '{0}'.",
                                                 updateTask.AddressBook.Name));
                }
                finally
                {
                    updateTask.RaiseFinished();

                    if (result)
                        _contactsManager.RaiseAddressBookReloaded(updateTask.AddressBook);

                    using (new EnsuredResourceCriticalOperation((object)_tasks))
                        _tasks[updateTask.Index] = null;
                }

                var endTime = DateTime.Now;
                Logger.LogNotice(string.Format("Updating contacts finised. Done in {0} seconds.", (endTime - updateTask.StartTime).TotalSeconds));
            }
        }
    }
}
