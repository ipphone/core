using System;
using System.ComponentModel;
using ContactPoint.Common;

namespace ContactPoint.Core
{
    public static class CoreLoader
    {
        public static event Action<string> PartLoading;
        public static event Action<Exception> LoadingFailed;

        public static ICore CreateCore(ISynchronizeInvoke syncInvoke)
        {
            try
            {
                RaisePartLoadingEvent("Initializing Core");
                Core.PartLoading += RaisePartLoadingEvent;

                return new Core(syncInvoke);
            }
            catch (Exception e)
            {
                LoadingFailed?.Invoke(e);
                throw;
            }
        }

        private static void RaisePartLoadingEvent(string partName)
        {
            PartLoading?.Invoke(partName);
        }
    }
}
