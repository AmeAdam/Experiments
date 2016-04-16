using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AmeCommon.MediaTasks.Settings
{
    [XmlRoot("card-profiles", Namespace = AmeNamespace)]
    [Serializable]
    public class AmeSettings
    {
        public const string AmeNamespace = "http://kamerzysta.bydgoszcz.pl";

        [XmlElement("svn-settings")]
        public SvnSettings SvnSettings { get; set; }

        [XmlElement("media")]
        public List<MediaSettings> Medias { get; set; }
    }
}