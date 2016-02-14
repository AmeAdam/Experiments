using System;
using System.Xml.Serialization;

namespace CardGrabberCmd.MediaTasks.Settings
{
    [XmlRoot("ame-card", Namespace = CardProfilesSettings.AmeNamespace)]
    [Serializable]
    public class AmeCardSettings
    {
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("index")]
        public string Index { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
