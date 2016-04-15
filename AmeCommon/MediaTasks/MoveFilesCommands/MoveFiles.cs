using System.Collections.Generic;

namespace AmeCommon.MediaTasks.MoveFilesCommands
{
    public class MoveFiles : IIOCommand
    {
        private readonly string targetFolderName;
        private readonly IEnumerable<string> filesList;

        public MoveFiles(IEnumerable<string> filesList, string targetFolderName)
        {
            this.targetFolderName = targetFolderName;
            this.filesList = filesList;
        }

        public void Execute(DestinationDirectory destinationDirectory)
        {
            destinationDirectory
                .GetChildDirectory(targetFolderName)
                .MoveAllFiles(filesList);
        }
    }
}
