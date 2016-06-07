using System;
using System.IO;
using System.Xml.Linq;

namespace AmeWindowsService.Config
{
    internal interface ISettingsProvider
    {
        XElement Root { get; }
        void Save();
    }

    internal class SettingsProvider : ISettingsProvider
    {
        private readonly object sync = new object();
        private readonly XDocument doc;
        private readonly string settingsPath;

        public SettingsProvider(string settingsPath = null)
        {
            this.settingsPath = settingsPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
            doc = XDocument.Load(this.settingsPath);
        }

        public XElement Root => doc.Root;

        public void Save()
        {
            lock (sync)
            {
                doc.Save(settingsPath);
            }
        }
    }
}
