using System.IO;
using AmeCommon.MediaTasks.Settings;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveZoom : BaseMediaTask
    {
        private readonly Media parent;
        private const string RelativeSource = @"STEREO";
        private readonly string relativeTarget;

        public MoveZoom(Media parent, TaskSettings settings)
        {
            this.parent = parent;
            relativeTarget = settings.GetParamValue("target");
        }

        public override void Execute()
        {
            var absoluteTarget = Path.Combine(parent.DestinationFolder.FullName, relativeTarget);
            var absoluteSource = Path.Combine(parent.SourceDisk.Name, RelativeSource);

            foreach (var chilSourcedDir in Directory.GetDirectories(absoluteSource))
                MoveAllFiles(chilSourcedDir, absoluteTarget);
        }
    }
}