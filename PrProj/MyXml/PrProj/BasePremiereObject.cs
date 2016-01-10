using System.Xml.Linq;

namespace MyXml.PrProj
{
    public class BasePremiereObject
    {
        public PremiereProject Project;
        public PremiereObjectKey Key { get; set; } 
        public XElement Element { get; set; }
    }
}