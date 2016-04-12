using System.IO;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveZoom : BaseMediaTask
    {
        private readonly string absoluteTargetPath;

        public MoveZoom(DirectoryInfo destinationDirectory, DriveInfo sourceDisk, string targetFolderName)
            : base(destinationDirectory, sourceDisk, "STEREO")
        {
            absoluteTargetPath = GetTargetPath(targetFolderName);
        }

        public override void Execute()
        {
            foreach (var chilSourcedDir in Directory.GetDirectories(RootSourceDirectory))
                MoveAllFiles(chilSourcedDir, absoluteTargetPath);
        }
    }
}