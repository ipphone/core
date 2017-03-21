using System.Collections.Generic;
using System.Xml.XPath;
using ContactPoint.Core.Settings.DataStructures;

namespace ContactPoint.Core.Settings.Loaders
{
    internal interface ISettingsLoader
    {
        IEnumerable<SettingsManagerSection> Load(XPathNavigator nav);
        object DeserializeRawItem(SettingsRawItem rawItem);
    }
}
