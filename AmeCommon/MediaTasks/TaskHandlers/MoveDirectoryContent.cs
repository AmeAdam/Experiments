using System.IO;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveDirectoryContent : IIOCommand
    {
        private string sourceDirectoryPath;
        private string targetFolderName;

        public MoveDirectoryContent(string sourceDirectoryPath, string targetFolderName)
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
                var childDestination = destinationDirectory.GetChildDirectory(Path.GetFileName(childDir));
                ExecuteInternal(childDir, childDestination);
                Directory.Delete(childDir);
            }
        }
    }
}
