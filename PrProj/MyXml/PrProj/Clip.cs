using System.Xml.Linq;

namespace MyXml.PrProj
{
    public class Clip
    {
        public XElement Element { get; set; }
        public string ClipID { get; set; }
        public string SourceRef { get; set; }
        public int FrameRate { get; set; }
    }
}
