using System;
using System.Threading;

namespace ContactPoint.Common
{
    public class ResourceCriticalOperation : IDisposable
    {
        private readonly int _timeout;
        public const int DEFAULT_DELAY_TIME_MS = 200;
        public const int INFINITE_TIMEOUT = 1000000;

        private int _lockedResourcesCount = 0;
        private object[] _lockedResources = null;

        public ResourceCriticalOperation(params object[] resources)
            : this(DEFAULT_DELAY_TIME_MS, resources)
        { }

        public ResourceCriticalOperation(int timeout, params object[] resources)
        {
            _timeout = timeout;
            _lockedResources = new object[resources.Length];

            foreach (var resource in resources)
            {
                if (Lock(resource))
                    _lockedResources[_lockedResourcesCount++] = resource;
                else
                {
                    var exception =
                        new TimeoutException(
                            string.Format("Cannot get access to locked resource of type {0} - request timed out.",
                                          resource.GetType()));
                    Logger.LogWarn(exception);

                    throw exception;
                }
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < _lockedResourcesCount; i++)
            {
                Monitor.Exit(_lockedResources[i]);

                _lockedResources[i] = null;
            }

            _lockedResourcesCount = 0;
            _lockedResources = null;
        }

        protected virtual bool Lock(object resource)
        {
            return Monitor.TryEnter(resource, _timeout);
        }
    }

    public class EnsuredResourceCriticalOperation : ResourceCriticalOperation
    {
        public EnsuredResourceCriticalOperation(params object[] resources)
            : base(resources)
        { }

        protected override bool Lock(object resource)
        {
            Monitor.Enter(resource);

            return true;
        }
    }
}
