using System.IO;
using CardGrabberCmd.MediaTasks.Settings;

namespace CardGrabberCmd.MediaTasks.TaskHandlers
{
    public class MoveAvchd : BaseMediaTask
    {
        private readonly Media parent;
        private const string RelativeSource = @"PRIVATE\AVCHD";
        private readonly string relativeTarget;


        public MoveAvchd(Media parent, TaskSettings settings)
        {
            this.parent = parent;
            relativeTarget = settings.GetParamValue("target");
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