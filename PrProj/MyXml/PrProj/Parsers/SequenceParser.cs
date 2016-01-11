using System.Xml.Linq;

namespace MyXml.PrProj.Parsers
{
    [PremiereElement("Sequence")]
    internal class SequenceParser : IPremiereElementParser
    {
        public BasePremiereObject Parse(XElement elem, PremiereProject project)
        {
            var key = new PremiereObjectKey
            {
                Name = elem.Name.LocalName,
                ObjectID = (int)elem.Element("ID"),
            };

            var res = project.AddOrGet<Sequence>(key);
            res.Element = elem;
            res.Name = (string)elem.Element("Name");
            res.Project = project;
            project.Add(res);
            return res;
        }
    }
}