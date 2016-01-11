using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MyXml
{
    public class ClipProj
    {
        public string Name;
        public List<XElement> TrackItems = new List<XElement>();

        public ClipProj(string name)
        {
            Name = name;
        }

        public void AddTrack(XElement elem)
        {
            var track = elem.Elements("ClipTrackItem").Elements("TrackItem").FirstOrDefault();
            if (track == null)
                return;
            TrackItems.Add(track);
        }

        public void UpdateTimes(long startFrame, long endFrame)
        {
            foreach (XElement elem in TrackItems)
            {
                var start = elem.Element("Start");
                if (start == null)
                    continue;
                var startNum = (startFrame * PrProjFile.Multiply);
                var end = elem.Element("End");
                if (end != null)
                {
                    var endNum = CalculateEnd(startNum, start.Value, end.Value);

                    end.Value = endNum.ToString(CultureInfo.InvariantCulture);
                }
                start.Value = startNum.ToString(CultureInfo.InvariantCulture);
            }
        }

        private long CalculateEnd(long newStart, string start, string end)
        {
            return newStart + long.Parse(end, CultureInfo.InvariantCulture) - long.Parse(start, CultureInfo.InvariantCulture);
        }
    }
}