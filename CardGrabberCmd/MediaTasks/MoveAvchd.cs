using System.IO;
using System.Xml.Linq;

namespace CardGrabberCmd.MediaTasks
{
    public class MoveAvchd : BaseMediaTask
    {
        private readonly Media parent;
        private const string RelativeSource = @"PRIVATE\AVCHD";
        private readonly string relativeTarget;


        public MoveAvchd(Media parent, XElement settings)
        {
            this.parent = parent;
            relativeTarget = (string)settings.Element(XName.Get("target", Media.AmeNamespace));
        }

        public override void Execute()
        {
            var absoluteTarget = Path.Combine(parent.DestinationFolder.FullName, relativeTarget);
            var absoluteSource = Path.Combine(parent.SourceDisk.Name, RelativeSource);
            Directory.CreateDirectory(absoluteTarget);
            MoveAllDirectoryContent(absoluteSource, absoluteTarget);
        }
    }
}