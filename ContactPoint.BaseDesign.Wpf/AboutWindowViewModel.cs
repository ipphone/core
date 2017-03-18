using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Security;
using Microsoft.Win32;

namespace ContactPoint.BaseDesign.Wpf
{
    public class AboutWindowViewModel : ViewModel
    {
        public string Version { get; private set; }
        public IEnumerable<PluginInformationViewModel> Plugins { get; private set; }
        public SecurityLicenseContent License { get; private set; }
        public string MachineId { get; private set; }
        public string ImageUri { get; private set; }

        public AboutWindowViewModel(ICore core)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ContactPoint\IpPhone"))
                {
                    if (key != null)
                    {
                        var licensePack = (byte[]) key.GetValue("LicenseObject");
                        License = SecurityLicenseProvider.GetLicense(licensePack);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Can't get license.");
            }

            ImageUri = Path.GetFullPath("partner_logo.png");
            Version = core.GetType().Assembly.GetName().Version.ToString(4);
            Plugins = core.PluginManager.Plugins.Select(x => new PluginInformationViewModel(x, License));
            MachineId = string.Join(":", SecurityLicenseProvider.GetMachineId());
        }
    }

    public class PluginInformationViewModel : ViewModel
    {
        public Guid ID { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string AssemblyName { get; private set; }
        public bool IsLicensed { get; private set; }
        public string LicenseExpires { get; private set; }
        public string Information { get; private set; }

        public PluginInformationViewModel(IPluginInformation plugin, SecurityLicenseContent license)
        {
            ID = plugin.ID;
            Name = plugin.Name;
            Version = plugin.Version;
            AssemblyName = plugin.AssemblyName;
            IsLicensed = false;
            Information = plugin.Info;
            LicenseExpires = "license is not applied.";

            if (license != null)
            {
                try
                {
                    var token =
                        license.Tokens.FirstOrDefault(
                            x =>
                            x.AssemblyKey.SequenceEqual(plugin.GetInstance().GetType().Assembly.GetName().GetPublicKey()));

                    if (token != null)
                    {
                        IsLicensed = true;
                        LicenseExpires = token.ExpireDate.HasValue ? token.ExpireDate.Value.ToString() : "never";
                    }
                }
                catch { }
            }
        }
    }
}
