using System;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    [Obsolete("Use PluginUIElementBase instead", false)]
    public abstract class PluginUIElement : PluginUIElementBase
    {
        public PluginUIElement(IPlugin plugin) : base(plugin)
        { }

        protected sealed override void ExecuteCommand(object sender, object data)
        {
            try
            {
                InternalExecute(sender, data);
            }
            catch (NotImplementedException)
            {
                try
                {
                    InternalExecute(sender);
                }
                catch (NotImplementedException)
                {
                    throw new NotImplementedException();
                }
            }
        }

        protected virtual void InternalExecute(object sender)
        {
            throw new NotImplementedException();
        }

        protected virtual void InternalExecute(object sender, object data)
        {
            throw new NotImplementedException();
        }
    }
}
