using AmeCommon.MediaTasks.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace AmeCommon.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        readonly AmeSettings settings;

        public SettingsProvider()
        {
            settings = LoadSettings<AmeSettings>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml"));
            if (File.Exists(UserSettingsFilePath))
                UserSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(UserSettingsFilePath));
            else
                UserSettings = new UserSettings();
        }

        private string UserSettingsFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AME", "user-settings.json");

        private static T LoadSettings<T>(string settingsFilePath)
        {
            var doc = XDocument.Load(settingsFilePath);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var xr = doc.CreateReader())
            {
                return (T)xs.Deserialize(xr);
            }
        }

        public void SaveUserSettings()
        {
            var filePath = UserSettingsFilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(UserSettings));
        }

        public SvnSettings SvnSettings => settings.SvnSettings;
        public List<CardSettings> CardSettings => settings.Medias;
        public UserSettings UserSettings { get; }
    }
}
