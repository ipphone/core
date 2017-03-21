using System.Threading;

namespace ContactPoint.Contacts
{
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