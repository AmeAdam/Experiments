using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CardGrabberCmd.MediaTasks.Settings
{
    [XmlRoot("card-profiles", Namespace = AmeNamespace)]
    [Serializable]
    public class CardProfilesSettings
    {
        public const string AmeNamespace = "http://kamerzysta.bydgoszcz.pl";

        [XmlElement("target-folder")]
        public List<TargetFolder> TargetFolders { get; set; }

        [XmlElement("media")]
        public List<MediaSettings> Medias { get; set; }
    }
}