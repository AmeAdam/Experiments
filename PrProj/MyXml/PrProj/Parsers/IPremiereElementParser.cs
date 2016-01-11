using System.Xml.Linq;

namespace MyXml.PrProj.Parsers
{
    public interface IPremiereElementParser
    {
        BasePremiereObject Parse(XElement elem, PremiereProject project);
    }
}