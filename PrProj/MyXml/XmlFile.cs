using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MyXml
{
    public class XmlFile
    {
        public const long Multiply = 10160640000L;
        private readonly XDocument doc;
        public List<ClipXml> Clips = new List<ClipXml>();

        public XmlFile(string filePath)
        {
            doc = XDocument.Load(filePath);
        }

        public void Analize()
        {
            if (doc.Root == null)
                return;
            var clips = doc.Root.DescendantNodes().OfType<XElement>().Where(e => e.Name == "clipitem");

            //var clips = doc.SelectNodes("xmeml/project/children/sequence/media/*/track/clipitem");
            //if (clips == null)
              //  return;
            foreach (var s in clips)
            {
                if (s.Element("start") == null)
                    continue;
                var name = GetPath(s);
                var start = GetLong(s, "start");
                var end = GetLong(s, "end");
                Clips.Add(new ClipXml(name, start, end));
            }
        }

        private string GetPath(XElement elem)
        {
            var pathNode = elem.Elements("file").Elements("pathurl").FirstOrDefault();
            if (pathNode == null)
                return null;
            var rawPath = ((string)pathNode).Replace("file://localhost/", "").ToLower();
            rawPath = rawPath.Replace("%3a", ":");
            rawPath = rawPath.Replace(@"file://?/", "");
            var path = System.Uri.UnescapeDataString(rawPath).Replace("/", @"\");
            return path.ToLower();
        }

        private string GetTrackIndex(XmlElement elem)
        {
            var index = elem.SelectSingleNode("link/trackindex");
            if (index == null)
                return null;
            return index.InnerText;
        }

        private long GetLong(XElement elem, string propName)
        {
            var a = (string)(elem.Element(propName));
            if (string.IsNullOrEmpty(a))
                return 0L;
            long res;
            long.TryParse(a, out res);
            return res;
        }

    }

    public class ClipXml
    {
        public string Name;
        public long Start;
        public long End;

        public ClipXml(string name, long start, long end)
        {
            Name = name;
            Start = start;
            End = end;
        }
    }
}
