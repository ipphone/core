using System;

namespace ContactPoint.Common.PluginManager
{
    public interface IPluginInformation : IService
    {
        Guid ID { get; }
        string Name { get; }
        string Version { get; }
        string AssemblyName { get; }
        string FileName { get; }
        string Info { get; }
        int Priority { get; }
        bool HaveSettingsForm { get; }
        void ShowSettingsDialog();
        IPlugin GetInstance(bool create = true);
    }
}
