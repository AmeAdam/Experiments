using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AmeCommon.MediaTasks.Settings
{
    [Serializable]
    public class MediaSettings
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("command")]
        public List<CommandSettings> Tasks { get; set; }
    }
}