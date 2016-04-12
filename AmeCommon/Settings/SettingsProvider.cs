using AmeCommon.MediaTasks.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AmeCommon.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        CardProfilesSettings profiles;

        public SettingsProvider()
        {
            var doc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cards-profiles.xml"));
            XmlSerializer xs = new XmlSerializer(typeof(CardProfilesSettings));
            using (var xr = doc.CreateReader())
            {
                profiles = (CardProfilesSettings)xs.Deserialize(xr);
            }

            var str = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            JsonConvert.DeserializeObject<CardProfilesSettings>(str);
            File.WriteAllText("d:\\json.test", str);
        }

        public SvnSettings SvnSettings { get { return profiles.SvnSettings; } }
        public List<MediaSettings> Medias { get { return profiles.Medias; } }
    }
}
