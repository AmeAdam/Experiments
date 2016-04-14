using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AmeCommon.MediaTasks.TaskHandlers
{
    public class MoveFiles : IIOCommand
    {
        private string targetFolderName;
        private string filePattern;
        private List<string> sourceDirectories = new List<string>();

        public MoveFiles(string sourcePathPattern, string targetFolderName)
        {
            this.targetFolderName = targetFolderName;
            var splittedPattern = sourcePathPattern.Split('\\', '/');
            if (splittedPattern.Length < 2)
                return;

            filePattern = splittedPattern.Last();
            sourceDirectories = new List<string> { splittedPattern[0] };
            for (int i = 1; i < splittedPattern.Length - 1; i++)
                sourceDirectories = GetMatchedDirectories(sourceDirectories, splittedPattern[i]);
        }

        private List<string> GetMatchedDirectories(List<string> sourceDirectories, string pattern)
        {
            var result = new List<string>();
            foreach (var dir in sourceDirectories)
                result.AddRange(Directory.GetDirectories(dir, pattern));
            return result;
        }

        public void Execute(DestinationDirectoryHandler destinationDirectory)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                destinationDirectory
                    .GetChildDirectory(targetFolderName)
                    .MoveAllFiles(sourceDirectory, filePattern);
            }
        }
    }
}
