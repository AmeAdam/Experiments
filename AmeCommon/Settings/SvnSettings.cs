using System;
using System.Xml.Serialization;

namespace AmeCommon.MediaTasks.Settings
{
    [Serializable]
    public class SvnSettings
    {
        [XmlElement("rootPath")]
        public string RootPath { get; set; }
        [XmlElement("user")]
        public string User { get; set; }
        [XmlElement("password")]
        public string Password { get; set; }
    }
}
