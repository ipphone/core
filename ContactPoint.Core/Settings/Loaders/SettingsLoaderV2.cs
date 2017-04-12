using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Collections;
using ContactPoint.Common;
using ContactPoint.Core.Settings.DataStructures;
using System.ComponentModel;

namespace ContactPoint.Core.Settings.Loaders
{
    internal class SettingsLoaderV2 : ISettingsLoader
    {
        private SettingsManager _settingsManager;

        public SettingsLoaderV2(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public virtual IEnumerable<SettingsManagerSection> Load(XPathNavigator nav)
        {
            XPathNodeIterator iter = nav.Select("/data/item");
            if (iter != null)
            {
                return new SettingsManagerSection[] { LoadSection("temporary", iter) };
            }

            return new SettingsManagerSection[] { };
        }

        public object DeserializeRawItem(SettingsRawItem rawItem)
        {
            if (rawItem.IsCollection) return DeserializeRawCollectionItem(rawItem);

            var type = GetTypeByName(rawItem.Type, true);
            if (type == null)
            {
                return null;
            }

            if (type == typeof(int)) return int.Parse(rawItem.Value);
            if (type == typeof(double)) return double.Parse(rawItem.Value);
            if (type == typeof(float)) return float.Parse(rawItem.Value);
            if (type == typeof(DateTime)) return DateTime.ParseExact(rawItem.Value, "s", null);
            if (type == typeof(bool)) return bool.Parse(rawItem.Value);
            if (type == typeof(string)) return rawItem.Value;
            if (type == typeof(char)) return rawItem.Value[0];

            var typeAttributes = type.GetCustomAttributes(typeof(TypeConverterAttribute), true)?
                .Select(x => x as TypeConverterAttribute)
                .Where(x => x != null && x != TypeConverterAttribute.Default && !string.IsNullOrEmpty(x.ConverterTypeName))
                .Join(type.GetCustomAttributesData(), x => x.GetType(), x => x.AttributeType, (x, y) => new { Attribute = x, Data = y });

            var typeConverterAttributes = typeAttributes
                .Select(x => (TypeConverterAttribute)x.Data.Constructor.Invoke(x.Data.ConstructorArguments.Select(p => p.Value).ToArray()))
                .Where(x => x != null);

            var typeConverterTypes = typeConverterAttributes
                .Select(x => GetTypeByName(x.ConverterTypeName))
                .Where(x => x != null);

            var typeConverters = typeConverterTypes
                .Select(x => (TypeConverter)Activator.CreateInstance(x))
                .Where(x => x != null);

            if (typeConverters == null)
            {
                return null;
            }

            foreach (var converter in typeConverters)
            {
                try
                {
                    return converter.ConvertFromString(rawItem.Value);
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, $"Converter of type '{converter.GetType().FullName}' was failed to convert '{rawItem.Value ?? "<NULL>"}' of type string");
                }
            }

            return null;
        }

        private object DeserializeRawCollectionItem(SettingsRawItem rawItem)
        {
            try
            {
                var itemType = GetTypeByName(rawItem.ItemType);
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

        private Type GetTypeByName(string typeName, bool throwOnError = true)
        {
            return Type.GetType(typeName, false) ?? GetTypeByNameFromAllAssemblies(typeName, throwOnError);
        }

        private Type GetTypeByNameFromAllAssemblies(string typeName, bool throwOnError = true)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.FullName.Equals(typeName, StringComparison.InvariantCulture));
            }
            catch
            {
                Logger.LogWarn($"Failed to get type '{typeName}', throwOnError={throwOnError}");
                if (throwOnError)
                {
                    throw;
                }

                // Try to remove assembly version from type name
                var typeNameParts = typeName.Split(',').Select(x => x.Trim()).ToArray();
                if (typeNameParts.Length > 2)
                {
                    return GetTypeByName(string.Join(", ", typeNameParts.Take(2)), false);
                }
            }

            return null;
        }
    }
}
