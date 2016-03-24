using System;
using System.Xml.Serialization;

namespace AmeCommon.MediaTasks.Settings
{
    [Serializable]
    public class TargetFolder
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("svn-ignore")]
        public string SvnIgnore { get; set; }
    }
}