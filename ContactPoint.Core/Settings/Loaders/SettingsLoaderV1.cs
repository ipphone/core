using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using ContactPoint.Common;
using System.IO;
using System.Xml.Serialization;
using ContactPoint.Core.Settings.DataStructures;

namespace ContactPoint.Core.Settings.Loaders
{
    internal class SettingsLoaderV1 : ISettingsLoader
    {
        private SettingsManager _settingsManager;

        public SettingsLoaderV1(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public IEnumerable<SettingsManagerSection> Load(XPathNavigator nav)
        {
            List<SettingsRawItem> rawData = new List<SettingsRawItem>();

            XPathNodeIterator iter = nav.Select("/data/item");
            if (iter != null)
            {
                while (iter.MoveNext())
                {
                    try
                    {
                        var item = new SettingsRawItem();

                        item.Name = iter.Current.GetAttribute("name", "");
                        item.Type = iter.Current.GetAttribute("type", "");
                        item.Value = iter.Current.Value;

                        rawData.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarn("Can't parse settings item value: " + ex.Message);
                    }
                }
            }

            return new SettingsManagerSection[] { new SettingsManagerSection("temporary", _settingsManager, this) };
        }

        public object DeserializeRawItem(SettingsRawItem rawItem)
        {
            try
            {
                StringReader reader = new StringReader(rawItem.Value);
                Type type = Type.GetType(rawItem.Type, true);

                XmlSerializer serializer = new XmlSerializer(type);
                object obj = serializer.Deserialize(reader);
                reader.Close();

                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogWarn("Can't deserialize setting from string: " + rawItem.Value + "; exception: " + ex.Message);
            }

            return null;
        }
    }
}
