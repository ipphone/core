using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ContactPoint.Common;

namespace ContactPoint.Core
{
    public static class CoreServiceExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<ICore, object>> ServiceFactories = new ConcurrentDictionary<Type, Func<ICore, object>>();

        public static void AddServiceFactory<TService>(this ICore core, Func<ICore, TService> serviceFactory) where TService : class
        {
            ServiceFactories.AddOrUpdate(typeof(TService), serviceFactory, (type, factory) => serviceFactory);
        }

        public static TService GetService<TService>(this ICore core) where TService : class
        {
            Func<ICore, object> serviceFactory;
            if (ServiceFactories.TryGetValue(typeof(TService), out serviceFactory))
            {
                var service = serviceFactory(core);
                if (service != null)
                {
                    var tservice = service as TService;
                    if (tservice != null)
                    {
                        return tservice;
                    }

                    throw new InvalidCastException($"Service factory for '${typeof(TService).FullName}' returned instance of '${service.GetType().FullName}'");
                }

                throw new InvalidOperationException($"Service factory for '${typeof(TService).FullName}' returned NULL");
            }

            throw new KeyNotFoundException($"Service factory for '${typeof(TService).FullName}' couldn't be found");
        }
    }
}
