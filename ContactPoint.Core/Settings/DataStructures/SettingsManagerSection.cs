using System;
using System.Collections;
using System.Collections.Generic;
using ContactPoint.Common;
using ContactPoint.Core.Settings.Loaders;

namespace ContactPoint.Core.Settings.DataStructures
{
    internal class SettingsManagerSection : ISettingsManagerSection
    {
        private SettingsManager _settingsManager;
        private IEnumerable<SettingsRawItem> _rawData;
        private ISettingsLoader _loader;
        protected IDictionary<string, object> Settings = new Dictionary<string, object>();

        internal bool IsEmpty
        {
            get
            {
                if (!IsLoaded) return true;

                return Settings.Count == 0;
            }
        }

        internal string Name { get; private set; }
        internal bool IsLoaded { get; private set; }

        internal SettingsManagerSection(string name, SettingsManager settingsManager, ISettingsLoader loader)
            : this(name, settingsManager, loader, new List<SettingsRawItem>())
        { }

        internal SettingsManagerSection(string name, SettingsManager settingsManager, ISettingsLoader loader, IEnumerable<SettingsRawItem> rawData)
        { 
            Name = name;
            _settingsManager = settingsManager;
            _loader = loader;
            _rawData = rawData;
        }

        internal virtual IEnumerable<SettingsRawItem> GetRawData()
        {
            if (!IsLoaded) return _rawData;

            var rawData = new List<SettingsRawItem>();

            lock (Settings)
            {
                foreach (KeyValuePair<string, object> item in Settings)
                    if (item.Value != null)
                        rawData.Add(CreateRawItem(item.Key, item.Value));
            }

            return rawData;
        }

        protected SettingsRawItem CreateRawItem(string name, object value)
        {
            var rawItem = new SettingsRawItem();

            rawItem.Name = name;
            rawItem.Type = value.GetType().FullName;

            if (value is IEnumerable && !(value is String))
            {
                rawItem.IsCollection = true;

                Type itemType;
                if (!value.GetType().IsGenericType) itemType = typeof(object);
                else itemType = value.GetType().GetGenericArguments()[0];

                rawItem.ItemType = itemType.FullName;

                foreach (var collectionItem in value as IEnumerable)
                    rawItem.ValuesCollection.Add(SerializeObject(collectionItem, itemType));
            }
            else
                rawItem.Value = SerializeObject(value, value.GetType());

            return rawItem;
        }

        protected string SerializeObject(object obj, Type type)
        {
            if (type == typeof(DateTime)) return ((DateTime)obj).ToString("s");
            if (type == typeof(System.Drawing.Point)) return new System.Drawing.PointConverter().ConvertToString(obj);

            return obj.ToString();
        }

        private void LoadSettings()
        {
            lock (Settings)
            {
                Settings.Clear();

                foreach (var item in _rawData)
                    Settings.Add(item.Name, _loader.DeserializeRawItem(item));

                IsLoaded = true;
            }
        }

        #region ISettingsManager Members

        public object this[string name]
        {
            get { return this.Get(name); }
            set { this.Set(name, value); }
        }

        public virtual T GetValueOrSetDefault<T>(string name, T defaultValue)
        {
            if (!IsLoaded) LoadSettings();

            lock (this.Settings)
            {
                if (this.Settings.ContainsKey(name) && this.Settings[name] is T)
                    return (T)this.Settings[name];
                else
                    this.Set(name, defaultValue);
            }

            return defaultValue;
        }

        public object GetValueOrSetDefault(string name, object defaultValue)
        {
            return GetValueOrSetDefault<object>(name, defaultValue);
        }

        public virtual T Get<T>(string name)
        {
            if (!IsLoaded) LoadSettings();

            lock (this.Settings)
            {
                if (this.Settings.ContainsKey(name) && this.Settings[name] is T)
                    return (T)this.Settings[name];
                else
                    return default(T);
            }
        }

        public object Get(string name)
        {
            return this.Get<object>(name);
        }

        public virtual void Set(string name, object value)
        {
            if (!IsLoaded) LoadSettings();

            lock (this.Settings)
            {
                if (this.Settings.ContainsKey(name))
                    this.Settings[name] = value;
                else
                    this.Settings.Add(name, value);
            }

            _settingsManager.SaveDeferred();
        }

        public void Save()
        {
            _settingsManager.Save();
        }

        #endregion
    }
}
