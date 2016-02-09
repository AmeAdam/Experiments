using System.IO;
using System.Xml.Linq;

namespace CardGrabberCmd.MediaTasks
{
    public class MoveDcim : BaseMediaTask
    {
        private readonly Media parent;
        private const string RelativeSource = @"DCIM";
        private readonly string relativeTarget;

        public MoveDcim(Media parent, XElement settings)
        {
            this.parent = parent;
            relativeTarget = (string)settings.Element(XName.Get("target", Media.AmeNamespace));
        }

        public override void Execute()
        {
            var absoluteTarget = Path.Combine(parent.DestinationFolder.FullName, relativeTarget);
            var absoluteSource = Path.Combine(parent.SourceDisk.Name, RelativeSource);
            MoveAllFiles(absoluteSource, absoluteTarget);
            foreach (var chilSourcedDir in Directory.GetDirectories(absoluteSource))
            {
                MoveAllDirectoryContent(chilSourcedDir, absoluteTarget);
            }
        }
    }
}