using System;
using System.Xml.Serialization;

namespace AmeCommon.MediaTasks.Settings
{
    [Serializable]
    public class CommandSettings
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("target")]
        public string Target { get; set; }
    }
}