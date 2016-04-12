using System;
using System.Collections.Generic;
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

        [XmlElement("svn-property")]
        public List<SvnPropertySettings> SvnProperties { get; set; }
    }

    [Serializable]
    public class SvnPropertySettings
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("value")]
        public string Value { get; set; }
        [XmlElement("path")]
        public string Path { get; set; }
    }
}
