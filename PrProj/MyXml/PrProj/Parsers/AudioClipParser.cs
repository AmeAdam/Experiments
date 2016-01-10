using System.Xml.Linq;

namespace MyXml.PrProj.Parsers
{
    [PremiereElement("AudioClip")]
    internal class AudioClipParser : IPremiereElementParser
    {
        public BasePremiereObject Parse(XElement elem, PremiereProject project)
        {
            var key = new PremiereObjectKey
            {
                Name = elem.Name.LocalName,
                ObjectID = (int)elem.Attribute("ObjectID"),
            };
            var res = project.AddOrGet<AudioClip>(key);

            res.Element = elem;
            res.Project = project;
            res.Clip = LoadClip(elem.Element("Clip"));
            project.Add(res);
            return res;
        }

        private Clip LoadClip(XElement elem)
        {
            if (elem == null)
                return null;
            return new Clip
            {
                Element = elem,
                ClipID = (string)elem.Element("ClipID"),
                FrameRate = (int)elem.Element("FrameRate"),
                SourceRef = (string)elem.Element("Source").Attribute("ObjectRef"),
            };
        }
    }
}
