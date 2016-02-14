using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CardGrabberCmd.MediaTasks.Settings
{
    [Serializable]
    public class MediaSettings
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("task")]
        public List<TaskSettings> Tasks { get; set; }
    }
}