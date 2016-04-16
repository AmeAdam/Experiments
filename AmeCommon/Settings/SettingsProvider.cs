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
        AmeSettings profiles;

        public SettingsProvider()
        {
            var doc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml"));
            XmlSerializer xs = new XmlSerializer(typeof(AmeSettings));
            using (var xr = doc.CreateReader())
            {
                profiles = (AmeSettings)xs.Deserialize(xr);
            }

            var str = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            JsonConvert.DeserializeObject<AmeSettings>(str);
            File.WriteAllText("d:\\json.test", str);
        }

        public SvnSettings SvnSettings { get { return profiles.SvnSettings; } }
        public List<MediaSettings> Medias { get { return profiles.Medias; } }
    }
}
