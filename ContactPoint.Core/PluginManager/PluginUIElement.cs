using System;
using ContactPoint.Common;
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
                Logger.LogNotice($"Failed to execute checked command for '{sender}', data is '{data}'");
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
            Logger.LogError("Invalid PluginUIElement instance - method 'InternalExecute(sender)' is not implemented");
            throw new NotImplementedException();
        }

        protected virtual void InternalExecute(object sender, object data)
        {
            Logger.LogError("Invalid PluginUIElement instance - method 'InternalExecute(sender, data)' is not implemented");
            throw new NotImplementedException();
        }
    }
}
