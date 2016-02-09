using System.Collections.Generic;
using System.Xml.Linq;

namespace CardGrabberCmd.MediaTasks
{
    public class MediaTaskFactory
    {
        public IEnumerable<IMediaTask> CreateAllTasks(XElement mediaElem, Media parent)
        {
            foreach (var avchd in mediaElem.Elements(XName.Get("move-avchd", Media.AmeNamespace)))
                yield return new MoveAvchd(parent, avchd);

            foreach (var dcim in mediaElem.Elements(XName.Get("move-dcim", Media.AmeNamespace)))
                yield return new MoveDcim(parent, dcim);

            foreach (var canon in mediaElem.Elements(XName.Get("move-dcim-canon", Media.AmeNamespace)))
                yield return new MoveDcimCanon(parent, canon);
        }
    }
}