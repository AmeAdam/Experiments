using System;
using System.Xml.Serialization;

namespace CardGrabberCmd.MediaTasks.Settings
{
    [Serializable]
    public class TaskParam
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}