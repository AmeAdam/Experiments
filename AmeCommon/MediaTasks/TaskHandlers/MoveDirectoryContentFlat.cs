using System.IO;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveDirectoryContentFlat : IIOCommand
    {
        private string sourceDirectoryPath;
        private string targetFolderName;

        public MoveDirectoryContentFlat(string sourceDirectoryPath, string targetFolderName)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.targetFolderName = targetFolderName;
        }

        public void Execute(DestinationDirectoryHandler destinationDirectory)
        {
            ExecuteInternal(sourceDirectoryPath, destinationDirectory.GetChildDirectory(targetFolderName));
        }

        private static void ExecuteInternal(string sourceDirectoryPath, DestinationDirectoryHandler destinationDirectory)
        {
            destinationDirectory.MoveAllFiles(sourceDirectoryPath);
            foreach (var childDir in Directory.GetDirectories(sourceDirectoryPath))
            {
                ExecuteInternal(childDir, destinationDirectory);
                Directory.Delete(childDir);
            }
        }
    }
}
