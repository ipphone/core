using System;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    [Obsolete("Use PluginCheckedUIElementBase instead", false)]
    public abstract class PluginCheckedUIElement : PluginCheckedUIElementBase
    {
        public PluginCheckedUIElement(IPlugin plugin) : base(plugin)
        { }

        protected sealed override void ExecuteCheckedCommand(object sender, bool checkedValue, object data)
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
