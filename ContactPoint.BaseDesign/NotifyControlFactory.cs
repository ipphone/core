using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ContactPoint.BaseDesign.BaseNotifyControls;

namespace ContactPoint.BaseDesign
{
    public class NotifyControlFactory
    {
        private static readonly ConcurrentDictionary<string, FactoryRegistration> factories = new ConcurrentDictionary<string, FactoryRegistration>();

        public static void RegisterNotifyControlFactory(string eventName, Func<object, NotifyControl> factory, bool @override = false)
        {
            factories.AddOrUpdate(eventName, k => new FactoryRegistration(factory), (k, v) => v.Add(factory, @override));
        }
        
        public static void UnregisterNotifyControlFactory(string eventName, Func<object, NotifyControl> factory)
        {
            factories.AddOrUpdate(eventName, k => new FactoryRegistration(), (k, v) => v.Remove(factory));
        }

        public static NotifyControl CreateNotifyControl(string eventName, object param)
        {
            if (factories.TryGetValue(eventName, out var registrations))
            {
                return registrations.CreateNotifyControl(param);
            }

            return null;
        }

        private class FactoryRegistration
        {
            private readonly List<Func<object, NotifyControl>> _registrations;

            public FactoryRegistration(params Func<object, NotifyControl>[] factories)
            {
                _registrations = new List<Func<object, NotifyControl>>(factories);
            }

            public FactoryRegistration Add(Func<object, NotifyControl> factory, bool @override = false)
            {
                if (@override)
                {
                    _registrations.Insert(0, factory);
                }
                else
                {
                    _registrations.Add(factory);
                }

                return this;
            }

            public FactoryRegistration Remove(Func<object, NotifyControl> factory)
            {
                _registrations.Remove(factory);
                return this;
            }

            public NotifyControl CreateNotifyControl(object param)
            {
                foreach (var factory in _registrations)
                {
                    var control = factory(param);
                    if (control != null)
                    {
                        return control;
                    }
                }

                return null;
            }
        }
    }
}
