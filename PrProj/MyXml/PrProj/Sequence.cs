using System.Collections.Generic;

namespace MyXml.PrProj
{
    public class Sequence : BasePremiereObject
    {
        public string Name { get; set; }
        public List<VideoSequenceSource> Sources = new List<VideoSequenceSource>();

        public override string ToString()
        {
            return Name;
        }
    }
}