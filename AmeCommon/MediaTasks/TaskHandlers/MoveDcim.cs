using System.IO;
using AmeCommon.MediaTasks.Settings;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveDcim : BaseMediaTask
    {
        private readonly Media parent;
        private const string RelativeSource = @"DCIM";
        private readonly string relativeTarget;

        public MoveDcim(Media parent, TaskSettings settings)
        {
            this.parent = parent;
            relativeTarget = settings.GetParamValue("target");
        }

        public override void Execute()
        {
            var absoluteTarget = Path.Combine(parent.DestinationFolder.FullName, relativeTarget);
            var absoluteSource = Path.Combine(parent.SourceDisk.Name, RelativeSource);
            MoveAllFiles(absoluteSource, absoluteTarget);
            foreach (var chilSourcedDir in Directory.GetDirectories(absoluteSource))
            {
                MoveAllDirectoryContent(chilSourcedDir, absoluteTarget, true);
            }
        }
    }
}