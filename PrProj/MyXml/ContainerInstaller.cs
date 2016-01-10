using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using MyXml.PrProj;
using MyXml.PrProj.Parsers;

namespace MyXml
{
    public class ContainerInstaller
    {
        public static void Install(IUnityContainer container)
        {
            container.RegisterType<PremiereProjectLoader>();
            RegisterPremiereXmlElementParsers(container);
        }

        private static void RegisterPremiereXmlElementParsers(IUnityContainer container)
        {
            var type = typeof (IPremiereElementParser);
            foreach (var parserType in type.Assembly.GetTypes().Where(t => type.IsAssignableFrom(t)))
            {
                var attr =
                    (PremiereElementAttribute)
                        Attribute.GetCustomAttribute(parserType, typeof (PremiereElementAttribute));
                if (attr != null)
                    container.RegisterType(type, parserType, attr.ElementName);
            }
        }
    }
}
