using System;
using ContactPoint.Common;
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
                Logger.LogNotice($"Failed to execute checked command for '{sender}', checked value is '{checkedValue}', data is '{data}'");
                try
                {
                    InternalExecute(sender);
                }
                catch (NotImplementedException)
                {
                    Logger.LogNotice($"Failed to re-execute checked command for '{sender}' only");
                    throw new NotImplementedException();
                }
            }
        }

        protected virtual void InternalExecute(object sender)
        {
            Logger.LogError("Invalid PluginCheckedUIElement instance - method 'InternalExecute(sender)' is not implemented");
            throw new NotImplementedException();
        }

        protected virtual void InternalExecute(object sender, object data)
        {
            Logger.LogError("Invalid PluginCheckedUIElement instance - method 'InternalExecute(sender, data)' is not implemented");
            throw new NotImplementedException();
        }
    }
}
