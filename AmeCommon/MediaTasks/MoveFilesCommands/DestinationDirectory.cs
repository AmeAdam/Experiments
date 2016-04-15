using System;
using System.Collections.Generic;
using System.IO;

namespace AmeCommon.MediaTasks.MoveFilesCommands
{
    public class DestinationDirectory
    {
        private readonly string destinationDirectory;

        public DestinationDirectory(string destinationDirectory)
        {
            this.destinationDirectory = destinationDirectory;
            Directory.CreateDirectory(destinationDirectory);
        }

        public DestinationDirectory GetChildDirectory(string childDirectoryName)
        {
            return new DestinationDirectory(Path.Combine(destinationDirectory, childDirectoryName));
        }

        public void MoveAllFiles(IEnumerable<string> sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                var destFileName = Path.GetFileName(sourceFile); 
                MoveFile(sourceFile, destFileName);
            }
        }

        private bool AreFilesSameByContent(string sourceFile, string destFile)
        {
            FileInfo src = new FileInfo(sourceFile);
            FileInfo dst = new FileInfo(destFile);
            if (src.Length != dst.Length)
                return false;

            using (FileStream fs1 = src.OpenRead())
            using (FileStream fs2 = dst.OpenRead())
            {
                while (true)
                {
                    var b1 = fs1.ReadByte();
                    var b2 = fs2.ReadByte();
                    if (b1 != b2)
                        return false;
                    if (b1 < 0)
                        return true;
                }
            }
        }

        public void MoveFile(string sourceFilePath, string destFileName)
        {
            var destFilePath = Path.Combine(destinationDirectory, destFileName);
            if (File.Exists(destFilePath))
            {
                if (AreFilesSameByContent(sourceFilePath, destFilePath))
                    File.Delete(sourceFilePath);
                else
                    File.Move(sourceFilePath, GetAlternativePath(destFileName));
            }
            else
                File.Move(sourceFilePath, destFilePath);
        }

        private string GetAlternativePath(string destFile)
        {
            var ext = Path.GetExtension(destFile);
            var name = Path.GetFileNameWithoutExtension(destFile);

            for (int i = 1; i < int.MaxValue; i++)
            {
                var newName = Path.Combine(destinationDirectory, name + "_" + i + ext);
                if (!File.Exists(newName))
                    return newName;
            }
            throw new ApplicationException("Unable to find alternative name for file "+destFile);
        }
    }
}
