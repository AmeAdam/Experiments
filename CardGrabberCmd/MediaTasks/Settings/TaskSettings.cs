using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CardGrabberCmd.MediaTasks.Settings
{
    [Serializable]
    public class TaskSettings
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("param")]
        public List<TaskParam> Params { get; set; }

        public string GetParamValue(string paramName)
        {
            var param = Params.FirstOrDefault(p => p.Name == paramName);
            if (param == null)
                throw new ApplicationException($"Param {paramName} not defined in task {Name}");
            return param.Value;
        }
    }
}