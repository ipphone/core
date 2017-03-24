using System.Threading;
using ContactPoint.Common;

namespace ContactPoint.Contacts
{
    public class EnsuredResourceCriticalOperation : ResourceCriticalOperation
    {
        public EnsuredResourceCriticalOperation(params object[] resources)
            : base(Lock, resources)
        { }

        private static bool Lock(object resource)
        {
            Monitor.Enter(resource);

            return true;
        }
    }
}