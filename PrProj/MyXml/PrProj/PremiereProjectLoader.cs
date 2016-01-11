using System.Xml.Linq;
using Microsoft.Practices.Unity;
using MyXml.PrProj.Parsers;

namespace MyXml.PrProj
{
    public class PremiereProjectLoader
    {
        private readonly IUnityContainer _container;

        public PremiereProjectLoader(IUnityContainer container)
        {
            _container = container;
        }

        public PremiereProject Load(string xmlFilePath)
        {
            return Load(XDocument.Load(xmlFilePath));
        }

        public PremiereProject Load(XDocument doc)
        {
            var res = new PremiereProject(doc);
            if (doc.Root == null)
                return res;
            foreach (var elem in doc.Root.Elements())
            {
                if (!_container.IsRegistered<IPremiereElementParser>(elem.Name.LocalName))
                    continue;
                var parser =_container.Resolve<IPremiereElementParser>(elem.Name.LocalName);
                parser.Parse(elem, res);
            }
            return res;
        }
    }
}
