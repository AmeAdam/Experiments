using System.IO;
using AmeCommon.MediaTasks.Settings;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveDcim : BaseMediaTask
    {
        private readonly string absoluteTargetPath;

        public MoveDcim(DirectoryInfo destinationDirectory, DriveInfo sourceDisk, string targetFolderName)
            : base(destinationDirectory, sourceDisk, "DCIM")
        {
            absoluteTargetPath = GetTargetPath(targetFolderName);
        }

        public override void Execute()
        {
            MoveAllFiles(RootSourceDirectory, absoluteTargetPath);
            foreach (var chilSourcedDir in Directory.GetDirectories(RootSourceDirectory))
            {
                MoveAllDirectoryContent(chilSourcedDir, absoluteTargetPath, true);
            }
        }
    }
}