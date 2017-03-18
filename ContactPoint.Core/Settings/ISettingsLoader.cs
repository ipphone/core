using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace ContactPoint.Core.Settings
{
    internal interface ISettingsLoader
    {
        IEnumerable<SettingsManagerSection> Load(XPathNavigator nav);
        object DeserializeRawItem(SettingsRawItem rawItem);
    }
}
