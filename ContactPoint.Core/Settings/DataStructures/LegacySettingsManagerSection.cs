using System.Collections.Generic;
using System.Collections.Specialized;
using ContactPoint.Core.Settings.Loaders;

namespace ContactPoint.Core.Settings.DataStructures
{
    internal class LegacySettingsManagerSection : SettingsManagerSection
    {
        private readonly StringCollection _ownKeys = new StringCollection();

        public LegacySettingsManagerSection(string name, SettingsManager settingsManager, ISettingsLoader loader, SettingsManagerSection parentSection)
            : base(name, settingsManager, loader, parentSection.GetRawData())
        { }

        public override IEnumerable<SettingsRawItem> GetRawData()
        {
            if (!IsLoaded) return new SettingsRawItem[] { };

            var rawData = new List<SettingsRawItem>();

            lock (Settings)
            {
                foreach (KeyValuePair<string, object> item in Settings)
                    if (_ownKeys.Contains(item.Key) && item.Value != null)
                        rawData.Add(CreateRawItem(item.Key, item.Value));
            }

            return rawData;
        }

        public override T Get<T>(string name)
        {
            if (!_ownKeys.Contains(name))
                _ownKeys.Add(name);

            return base.Get<T>(name);
        }

        public override T GetValueOrSetDefault<T>(string name, T defaultValue)
        {
            if (!_ownKeys.Contains(name))
                _ownKeys.Add(name);

            return base.GetValueOrSetDefault(name, defaultValue);
        }

        public override void Set(string name, object value)
        {
            if (!_ownKeys.Contains(name))
                _ownKeys.Add(name);

            base.Set(name, value);
        }
    }
}
