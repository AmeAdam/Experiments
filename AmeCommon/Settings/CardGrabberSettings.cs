using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AmeCommon.MediaTasks.Settings
{
    [XmlRoot("card-profiles", Namespace = AmeNamespace)]
    [Serializable]
    public class CardGrabberSettings
    {
        public const string AmeNamespace = "http://kamerzysta.bydgoszcz.pl";

        [XmlElement("svn-settings")]
        public SvnSettings SvnSettings { get; set; }

        [XmlElement("media")]
        public List<CardSettings> Medias { get; set; }
    }
}