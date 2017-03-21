using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Plugins.WebServer.Handlers;
using ContactPoint.Common;

namespace ContactPoint.Plugins.WebServer
{
    internal class HandlerManager
    {
        private Dictionary<string, IHandler> _handlers = new Dictionary<string, IHandler>();

        public IHandler this[string uri]
        {
            get 
            { 
                if (_handlers.ContainsKey(uri))
                    return _handlers[uri];

                return null;
            }
        }

        public HandlerManager()
        { }

        public void RegisterHandler(string uri, IHandler handler)
        {
            _handlers.Add(uri, handler);
        }

        public void UnregisterHandler(string uri)
        {
            _handlers.Remove(uri);
        }
    }
}
