using System;
using System.Collections.Generic;
using System.Xml.XPath;
using ContactPoint.Common;
using ContactPoint.Core.Settings.DataStructures;

namespace ContactPoint.Core.Settings.Loaders
{
    internal class SettingsLoaderV3  : SettingsLoaderV2
    {
        public SettingsLoaderV3(SettingsManager settingsManager)
            : base(settingsManager)
        { }

        public override IEnumerable<SettingsManagerSection> Load(XPathNavigator nav)
        {
            var sections = new List<SettingsManagerSection>();

            XPathNodeIterator iter = nav.Select("/data/section");
            if (iter != null)
            {
                while (iter.MoveNext())
                {
                    try
                    {
                        var name = iter.Current.GetAttribute("name", "");

                        var itemIter = iter.Current.Select("item");
                        if (itemIter != null)
                            sections.Add(LoadSection(name, itemIter));
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarn(e);
                    }
                }
            }

            return sections;
        }
    }
}
