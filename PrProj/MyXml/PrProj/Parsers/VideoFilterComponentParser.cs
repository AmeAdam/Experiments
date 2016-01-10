using System.Xml.Linq;

namespace MyXml.PrProj.Parsers
{
    [PremiereElement("VideoFilterComponent")]
    internal class VideoFilterComponentParser : IPremiereElementParser
    {
        public BasePremiereObject Parse(XElement elem, PremiereProject project)
        {
            var key = new PremiereObjectKey
            {
                Name = elem.Name.LocalName,
                ObjectID = (int) elem.Attribute("ObjectID"),
            };

            var res = project.AddOrGet<VideoFilterComponent>(key);

            res.Element = elem;
            res.Project = project;

            var component = elem.Element("Component");
            res.Component = component;
            if (component != null)
            {
                res.Name = (string) component.Element("DisplayName");
                res.Bypass = component.Element("Bypass");
            }
            project.Add(res);
            return res;
        }
     }
}
