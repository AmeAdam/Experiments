using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MyXml.PrProj.Parsers
{
    [PremiereElement("VideoComponentChain")]
    internal class VideoComponentChainParser : IPremiereElementParser
    {
        public BasePremiereObject Parse(XElement elem, PremiereProject project)
        {
            var seq = elem.Element("SeqID");
            if (seq == null)
                return null;

            var res = new VideoComponentChain
            {
                Key = new PremiereObjectKey
                {
                    Name = elem.Name.LocalName,
                    ObjectID = (int) elem.Attribute("ObjectID"),
                },
                Element = elem,
                Project = project,
                SeqId = (int)seq,
                components = LoadComponents(elem.Element("ComponentChain"), project)
            };
            project.Add(res);
            return res;
        }

        private List<VideoComponentChain.Component> LoadComponents(XElement chains, PremiereProject project)
        {
            if (chains == null)
                return new List<VideoComponentChain.Component>(0);
            return chains
                .Elements("Components")
                .Elements("Component")
                .Select(c => LoadComponent(c, project))
                .ToList();
        }

        private VideoComponentChain.Component LoadComponent(XElement component, PremiereProject project)
        {
            var videoFilterKey = new PremiereObjectKey
            {
                Name = "VideoFilterComponent",
                ObjectID = (int) component.Attribute("ObjectRef"),
            };

            return new VideoComponentChain.Component((int) component.Attribute("Index"))
            {
                Element = component,
                ObjectRef = (string) component.Attribute("ObjectRef"),
                ReferencedComponent = project.AddOrGet<VideoFilterComponent>(videoFilterKey)
            };
        }
    }
}
