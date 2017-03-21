using System;
using System.Collections.Generic;

namespace ContactPoint.Core.Settings.DataStructures
{
    internal class SettingsRawItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsCollection { get; set; }
        public string ItemType { get; set; }
        public string Value { get; set; }
        public List<string> ValuesCollection { get; private set; }

        public SettingsRawItem()
        {
            Name = String.Empty;
            Type = String.Empty;
            IsCollection = false;
            ItemType = String.Empty;
            Value = String.Empty;
            ValuesCollection = new List<string>();
        }
    }
}
