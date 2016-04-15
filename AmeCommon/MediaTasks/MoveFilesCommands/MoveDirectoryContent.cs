namespace AmeCommon.MediaTasks.MoveFilesCommands
{
    public class MoveDirectoryContent : IIOCommand
    {
        private readonly SourceDirectory sourceDirectory;
        private readonly string targetFolderName;

        public MoveDirectoryContent(SourceDirectory sourceDirectory, string targetFolderName)
        {
            this.sourceDirectory = sourceDirectory;
            this.targetFolderName = targetFolderName;
        }

        public void Execute(DestinationDirectory destinationDirectory)
        {
            ExecuteInternal(sourceDirectory, destinationDirectory.GetChildDirectory(targetFolderName));
        }

        private static void ExecuteInternal(SourceDirectory sourceDirectoryPath, DestinationDirectory destinationDirectory)
        {
            destinationDirectory.MoveAllFiles(sourceDirectoryPath.GetFiles());
            foreach (var childDir in sourceDirectoryPath.Childs)
            {
                var childDestination = destinationDirectory.GetChildDirectory(childDir.Name);
                ExecuteInternal(childDir, childDestination);
                childDir.TryDelete();
            }
        }
    }
}
