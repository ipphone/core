using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Collections;
using ContactPoint.Common;
using ContactPoint.Core.Settings.DataStructures;

namespace ContactPoint.Core.Settings.Loaders
{
    internal class SettingsLoaderV2 : ISettingsLoader
    {
        private readonly SettingsManager _settingsManager;

        public SettingsLoaderV2(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public virtual IEnumerable<SettingsManagerSection> Load(XPathNavigator nav)
        {
            return new[] { LoadSection("temporary", nav.Select("/data/item")) };
        }

        public object DeserializeRawItem(SettingsRawItem rawItem)
        {
            if (rawItem.IsCollection) return DeserializeRawCollectionItem(rawItem);

            var type = TypeHelpers.GetTypeByName(rawItem.Type);
            if (type == null)
            {
                return null;
            }

            if (type == typeof(byte)) return byte.Parse(rawItem.Value);
            if (type == typeof(int)) return int.Parse(rawItem.Value);
            if (type == typeof(long)) return long.Parse(rawItem.Value);
            if (type == typeof(double)) return double.Parse(rawItem.Value);
            if (type == typeof(float)) return float.Parse(rawItem.Value);
            if (type == typeof(DateTime)) return DateTime.ParseExact(rawItem.Value, "s", null);
            if (type == typeof(bool)) return bool.Parse(rawItem.Value);
            if (type == typeof(string)) return rawItem.Value;
            if (type == typeof(char)) return rawItem.Value[0];

            if (type == typeof(byte?)) return rawItem.Value != null ? new byte?(byte.Parse(rawItem.Value)) : null;
            if (type == typeof(int?)) return rawItem.Value != null ? new int?(int.Parse(rawItem.Value)) : null;
            if (type == typeof(long?)) return rawItem.Value != null ? new long?(long.Parse(rawItem.Value)) : null;
            if (type == typeof(double?)) return rawItem.Value != null ? new double?(double.Parse(rawItem.Value)) : null;
            if (type == typeof(float?)) return rawItem.Value != null ? new float?(float.Parse(rawItem.Value)) : null;
            if (type == typeof(DateTime?)) return rawItem.Value != null ? new DateTime?(DateTime.ParseExact(rawItem.Value, "s", null)) : null;
            if (type == typeof(bool?)) return rawItem.Value != null ? new bool?(bool.Parse(rawItem.Value)) : null;
            if (type == typeof(char?)) return rawItem.Value != null ? new char?(rawItem.Value[0]) : null;

            if (type.TryGetTypeConverter(out var typeConverter))
            {
                return typeConverter.ConvertFromString(rawItem.Value);
            }

            return null;
        }

        private object DeserializeRawCollectionItem(SettingsRawItem rawItem)
        {
            try
            {
                var itemType = TypeHelpers.GetTypeByName(rawItem.ItemType);
                var list = (IList)typeof(List<>).MakeGenericType(itemType).GetConstructor(new Type[] { }).Invoke(null);

                foreach (var item in rawItem.ValuesCollection)
                {
                    list.Add(item);
                }

                return list;
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't deserialize collection.");

                return null;
            }
        }

        protected SettingsManagerSection LoadSection(string name, XPathNodeIterator iter)
        {
            var rawData = new List<SettingsRawItem>();

            while (iter.MoveNext())
            {
                try
                {
                    var rawItem = new SettingsRawItem()
                    {
                        Name = iter.Current.GetAttribute("name", ""),
                        Type = iter.Current.GetAttribute("type", ""),
                        IsCollection = bool.Parse(iter.Current.GetAttribute("isCollection", ""))
                    };

                    if (!rawItem.IsCollection)
                    {
                        rawItem.Value = iter.Current.Value;
                    }
                    else
                    {
                        rawItem.ItemType = iter.Current.GetAttribute("itemType", "");

                        var collectionIter = iter.Current.Select("collectionItem");
                        while (collectionIter.MoveNext())
                        {
                            rawItem.ValuesCollection.Add(collectionIter.Current.Value);
                        }
                    }

                    rawData.Add(rawItem);
                }
                catch (Exception ex)
                {
                    Logger.LogWarn("Can't parse settings item value: " + ex.Message);
                }
            }

            return new SettingsManagerSection(name, _settingsManager, this, rawData);
        }
    }
}
