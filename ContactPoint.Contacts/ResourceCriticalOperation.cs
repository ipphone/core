using System;
using System.Threading;
using ContactPoint.Common;

namespace ContactPoint.Contacts
{
    public class ResourceCriticalOperation : IDisposable
    {
        public const int DEFAULT_DELAY_TIME_MS = 200;
        public const int INFINITE_TIMEOUT = 1000000;

        private int _lockedResourcesCount;
        private object[] _lockedResources;

        public Guid OperationId { get; } = Guid.NewGuid();

        public ResourceCriticalOperation(params object[] resources)
            : this(DEFAULT_DELAY_TIME_MS, resources)
        { }

        public ResourceCriticalOperation(int timeout, params object[] resources)
            : this (o => Monitor.TryEnter(o, timeout), resources)
        { }

        protected ResourceCriticalOperation(Func<object, bool> lockFunc, object[] resources)
        {
            Logger.LogNotice($"Starting Resource Critical Operation '{OperationId}'");

            _lockedResources = new object[resources.Length];

            foreach (var resource in resources)
            {
                if (lockFunc(resource))
                {
                    _lockedResources[_lockedResourcesCount++] = resource;
                }
                else
                {
                    var exception = new TimeoutException($"Cannot get access to locked resource of type {resource.GetType()} - request timed out.");
                    Logger.LogWarn(exception);

                    throw exception;
                }
            }
        }

        public void Dispose()
        {
            Logger.LogNotice($"Finalizing Resource Critical Operation '{OperationId}'");

            for (var i = 0; i < _lockedResourcesCount; i++)
            {
                Monitor.Exit(_lockedResources[i]);

                _lockedResources[i] = null;
            }

            _lockedResourcesCount = 0;
            _lockedResources = null;
        }
    }
}
