using ContactPoint.Common;
using Newtonsoft.Json.Linq;

namespace ContactPoint.Core.PluginManager
{
    class JObjectReadOnlySettingsManagerSection : ISettingsManagerSection
    {
        private readonly JObject _j;

        public object this[string name]
        {
            get { return _j.GetValue(name); }
            set { }
        }

        public JObjectReadOnlySettingsManagerSection(JObject j)
        {
            _j = j;
        }

        public T GetValueOrSetDefault<T>(string name, T defaultValue)
        {
            JToken token;
            return _j.TryGetValue(name, out token) ? token.Value<T>() : defaultValue;
        }

        public object GetValueOrSetDefault(string name, object defaultValue)
        {
            JToken token;
            return _j.TryGetValue(name, out token) ? token.Value<object>() : defaultValue;
        }

        public T Get<T>(string name)
        {
            return GetValueOrSetDefault(name, default(T));
        }

        public object Get(string name)
        {
            return GetValueOrSetDefault(name, null);
        }

        public void Set(string name, object value)
        { }

        public void Save()
        { }
    }
}