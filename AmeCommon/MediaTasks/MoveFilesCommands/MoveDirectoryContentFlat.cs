namespace AmeCommon.MediaTasks.MoveFilesCommands
{
    public class MoveDirectoryContentFlat : IIOCommand
    {
        private readonly SourceDirectory source;
        private readonly string targetFolderName;

        public MoveDirectoryContentFlat(SourceDirectory source, string targetFolderName)
        {
            this.source = source;
            this.targetFolderName = targetFolderName;
        }

        public void Execute(DestinationDirectory destinationDirectory)
        {
            ExecuteInternal(source, destinationDirectory.GetChildDirectory(targetFolderName));
        }

        private static void ExecuteInternal(SourceDirectory source, DestinationDirectory destination)
        {
            destination.MoveAllFiles(source.GetFiles());
            foreach (var childDir in source.Childs)
            {
                ExecuteInternal(childDir, destination);
                childDir.TryDelete();
            }
        }
    }
}
