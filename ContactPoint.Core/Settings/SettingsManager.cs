using System;
using System.Collections.Generic;
using ContactPoint.Common;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.XPath;
using ContactPoint.Core.Settings.DataStructures;
using ContactPoint.Core.Settings.Loaders;

namespace ContactPoint.Core.Settings
{
    internal class SettingsManager
    {
        private readonly List<SettingsManagerSection> _sections = new List<SettingsManagerSection>();
        private readonly string _fileName;
        private readonly Timer _deferredSaveTimer;
        private SettingsManagerSection _legacySection = null;

        public SettingsManager(Core core)
        {
            var oldCallServiceSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "callservice.settings.xml");
            var oldContactPointSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "contactpoint.settings.xml");

            if (File.Exists(oldCallServiceSettingsFile) && !File.Exists(oldContactPointSettingsFile))
                File.Move(oldCallServiceSettingsFile, oldContactPointSettingsFile);

            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "contactpoint");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var actualSettingsFile = Path.Combine(directoryPath, "contactpoint.settings.xml");
            if (File.Exists(oldContactPointSettingsFile))
            {
                if (!File.Exists(actualSettingsFile))
                    File.Move(oldContactPointSettingsFile, actualSettingsFile);
                else
                    File.Delete(oldContactPointSettingsFile);
            }

            _fileName = actualSettingsFile;

            Load();

            _deferredSaveTimer = new Timer();
            _deferredSaveTimer.Interval = 3000;
            _deferredSaveTimer.Tick += new EventHandler(DeferredSaveTimerTick);
        }

        public ISettingsManagerSection GetSection(string name)
        {
            SettingsManagerSection section;

            lock (_sections)
            {
                section = _sections.FirstOrDefault(x => x.Name == name);

                if (section == null)
                {
                    if (_legacySection == null) section = new SettingsManagerSection(name, this, new SettingsLoaderV3(this));
                    else section = new LegacySettingsManagerSection(name, this, new SettingsLoaderV3(this), _legacySection);

                    _sections.Add(section);
                }
            }

            return section;
        }

        public void Save()
        {
            if (_deferredSaveTimer.Enabled)
                _deferredSaveTimer.Stop();

            lock (_sections)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement dataNode = doc.CreateElement("data");

                AppendAttribute(dataNode, "version", "3");

                foreach (var section in _sections)
                {
                    // Prevent legacy section to save because all settings should be in another sections.
                    if (section == _legacySection) continue;

                    var node = doc.CreateElement("section");

                    AppendAttribute(node, "name", section.Name);

                    foreach (var item in section.GetRawData())
                    {
                        XmlNode itemNode = doc.CreateNode(XmlNodeType.Element, "item", "");

                        AppendAttribute(itemNode, "name", item.Name);
                        AppendAttribute(itemNode, "type", item.Type);
                        AppendAttribute(itemNode, "isCollection", item.IsCollection.ToString());

                        if (item.IsCollection)
                        {
                            AppendAttribute(itemNode, "itemType", item.ItemType);

                            foreach (var collectionItem in item.ValuesCollection)
                            {
                                XmlNode collectionItemNode = doc.CreateNode(XmlNodeType.Element, "collectionItem", "");
                                AppendValue(collectionItemNode, collectionItem);
                                itemNode.AppendChild(collectionItemNode);
                            }
                        }
                        else
                            AppendValue(itemNode, item.Value);

                        node.AppendChild(itemNode);
                    }

                    dataNode.AppendChild(node);
                }

                doc.AppendChild(dataNode);

                using (FileStream fileStream = File.Open(_fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    doc.Save(fileStream);
                    fileStream.Flush(true);
                }
            }
        }

        internal void SaveDeferred()
        {
            if (_deferredSaveTimer.Enabled)
                _deferredSaveTimer.Stop();

            _deferredSaveTimer.Start();
        }

        private void Load()
        {
            lock (_sections)
            {
                _sections.Clear();

                try
                {
                    using (FileStream fileStream = File.OpenRead(this._fileName))
                    {
                        XPathDocument document = new XPathDocument(fileStream);
                        XPathNavigator nav = document.CreateNavigator();

                        string version = String.Empty;
                        try
                        {
                            version = nav.SelectSingleNode("/data").GetAttribute("version", "");
                        }
                        catch { version = "1"; }

                        ISettingsLoader loader = null;

                        if (version == "1") loader = new SettingsLoaderV1(this);
                        else if (version == "2") loader = new SettingsLoaderV2(this);
                        else loader = new SettingsLoaderV3(this);

                        if (loader != null)
                            _sections.AddRange(loader.Load(nav));

                        // Add first section as legacy and it will switch SettingManager to legacy mode.
                        // This mode should be active only for valid conversion from older version
                        // where we doesn't have sections in settings file.
                        if ((version == "1" || version == "2") && _sections.Count == 1)
                            _legacySection = _sections[0];
                    }
                }
                catch (Exception e)
                {
                    Logger.LogNotice("Failed to load settings");
                    Logger.LogWarn(e);
                }
            }
        }

        private void DeferredSaveTimerTick(object sender, EventArgs e)
        {
            this._deferredSaveTimer.Stop();

            this.Save();
        }

        #region XML helpers

        private static void AppendAttribute(XmlNode node, string name, string value)
        {
            XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
            attribute.Value = value;
            node.Attributes.Append(attribute);
        }

        private static void AppendValue(XmlNode node, string serializedValue)
        {
            XmlNode valueNode = node.OwnerDocument.CreateNode(XmlNodeType.CDATA, "cditem", "");
            valueNode.Value = serializedValue;
            node.AppendChild(valueNode);
        }

        #endregion
    }
}
