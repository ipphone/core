namespace ContactPoint.Common
{
    public interface ISettingsManagerSection
    {
        /// <summary>
        /// Get setting value by name
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <returns>Value of setting</returns>
        object this[string name] { get; set; }

        /// <summary>
        /// Get setting value or set and return default
        /// </summary>
        /// <typeparam name="T">Type of setting</typeparam>
        /// <param name="name">Name of setting</param>
        /// <param name="defaultValue">Default value of setting</param>
        /// <returns>Setting value or default</returns>
        T GetValueOrSetDefault<T>(string name, T defaultValue);

        /// <summary>
        /// Get setting value or set and return default
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <param name="defaultValue">Default value of setting</param>
        /// <returns>Setting value or type default</returns>
        object GetValueOrSetDefault(string name, object defaultValue);

        /// <summary>
        /// Get setting value typed
        /// </summary>
        /// <typeparam name="T">Type of setting</typeparam>
        /// <param name="name">Name of setting</param>
        /// <returns>Setting value or type default</returns>
        T Get<T>(string name);

        /// <summary>
        /// Get setting value
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <returns>Setting value or type default</returns>
        object Get(string name);

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <param name="value">Value to set</param>
        void Set(string name, object value);

        /// <summary>
        /// Save settings into datasource
        /// </summary>
        void Save();
    }
}
