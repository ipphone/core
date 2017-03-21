using System;
using System.ComponentModel;

namespace ContactPoint.BaseDesign
{
    /// <summary>
    /// Providing static class for synchronizing with UI thread
    /// </summary>
    public static class SyncUi
    {
        private static ISynchronizeInvoke _syncObject = null;

        /// <summary>
        /// Initializing mechanism. Should be called from UI thread.
        /// Should be called once. Second calls will not affect object.
        /// </summary>
        /// <param name="syncObject">Object to synchronize with. Should be alive while program running.</param>
        public static void Initialize(ISynchronizeInvoke syncObject) => _syncObject = syncObject;

        public static bool InvokeRequired => _syncObject.InvokeRequired;

        public static object Invoke(Delegate method, params object[] args) => _syncObject.Invoke(method, args);
    }
}
