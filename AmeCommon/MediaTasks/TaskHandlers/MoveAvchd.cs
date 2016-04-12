using System.IO;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveAvchd : BaseMediaTask
    {
        private readonly string absoluteTargetPath;

        public MoveAvchd(DirectoryInfo destinationDirectory, DriveInfo sourceDisk, string targetFolderName)
            : base(destinationDirectory, sourceDisk, "PRIVATE\\AVCHD")
        {
            absoluteTargetPath = GetTargetPath(targetFolderName);
        }

        public override void Execute()
        {
            Directory.CreateDirectory(absoluteTargetPath);
            MoveAllDirectoryContent(RootSourceDirectory, absoluteTargetPath, false);
        }
    }
}