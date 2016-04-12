using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmeCommon.MediaTasks
{
    public class MoveDirectoryContent
    {
        private string rootSourceDirectory;
        private string rootDestinationDirectory;
        private FileOperation filesService;

        public MoveDirectoryContent(FileOperation filesService, string rootSourceDirectory, string rootDestinationDirectory)
        {
            this.rootSourceDirectory = rootSourceDirectory;
            this.rootDestinationDirectory = rootDestinationDirectory;
            this.filesService = filesService;
        }

        public void Execute()
        {
            ExecuteInternal(rootSourceDirectory, rootDestinationDirectory);
        }

        private void ExecuteInternal(string sourceDirectory, string destinationDirectory)
        {
            filesService.MoveAllFiles(sourceDirectory, destinationDirectory);
            foreach (var childDir in Directory.GetDirectories(sourceDirectory))
            {
                var childDirName = Path.GetFileName(childDir);
                ExecuteInternal(childDir, Path.Combine(destinationDirectory, childDirName));
                filesService.TryDeleteDirectory(childDir);
            }      
        }
    }

    public class MoveDirectoryContentFlat
    {
        private string rootSourceDirectory;
        private string rootDestinationDirectory;
        private FileOperation filesService;

        public MoveDirectoryContentFlat(FileOperation filesService, string rootSourceDirectory, string rootDestinationDirectory)
        {
            this.rootSourceDirectory = rootSourceDirectory;
            this.rootDestinationDirectory = rootDestinationDirectory;
            this.filesService = filesService;
        }

        public void Execute()
        {
            ExecuteInternal(rootSourceDirectory, rootDestinationDirectory);
        }

        private void ExecuteInternal(string sourceDirectory, string destinationDirectory)
        {
            filesService.MoveAllFiles(sourceDirectory, destinationDirectory);
            foreach (var childDir in Directory.GetDirectories(sourceDirectory))
            {
                ExecuteInternal(childDir, destinationDirectory);
                filesService.TryDeleteDirectory(childDir);
            }
        }
    }

    public class MoveFiles
    {
        private string filePattern;
        private string rootDestinationDirectory;
        private List<string> sourceDirectories = new List<string>();
        private FileOperation filesService;

        public MoveFiles(FileOperation filesService, string sourcePattern, string rootSourceDirectory, string rootDestinationDirectory)
        {
            this.filesService = filesService;

            if (Directory.Exists(rootSourceDirectory))
            {
                var splittedPattern = sourcePattern.Split('\\', '/');
                var filePattern = splittedPattern.Last();

                sourceDirectories = new List<string> { rootSourceDirectory };
                for (int i = 0; i < splittedPattern.Length - 1; i++)
                    sourceDirectories = GetMatchedDirectories(sourceDirectories, splittedPattern[i]);
            }

            this.rootDestinationDirectory = rootDestinationDirectory;
        }

        private List<string> GetMatchedDirectories(List<string> sourceDirectories, string pattern)
        {
            var result = new List<string>();
            foreach (var dir in sourceDirectories)
                result.AddRange(Directory.GetDirectories(dir, pattern));
            return result;
        }

        public void Execute()
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                filesService.MoveAllFiles(sourceDirectory, rootDestinationDirectory, filePattern);
            }
        }
    }

    public class FileOperation
    {
        public bool TryDeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return true;
            try
            {
                Directory.Delete(path);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public void MoveAllFiles(string sourceDirectory, string destinationDirectory, string searchPattern = null)
        {
            if (!Directory.Exists(sourceDirectory))
                return;

            Directory.CreateDirectory(destinationDirectory);

            var sourceFiles = searchPattern != null
                ? Directory.GetFiles(sourceDirectory, searchPattern)
                : Directory.GetFiles(sourceDirectory);


            foreach (var sourceFile in sourceFiles)
            {
                var destFile = Path.Combine(destinationDirectory, Path.GetFileName(sourceFile) ?? "");
                MoveFile(sourceFile, destFile);
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

        public void MoveFile(string sourceFile, string destFile)
        {
            if (File.Exists(destFile))
            {
                if (AreFilesSameByContent(sourceFile, destFile))
                    File.Delete(sourceFile);
                else
                    File.Move(sourceFile, GetAlternativeName(destFile));
            }
            else
                File.Move(sourceFile, destFile);
        }

        private string GetAlternativeName(string destFile)
        {
            var path = Path.GetDirectoryName(destFile);
            var ext = Path.GetExtension(destFile);
            var name = Path.GetFileNameWithoutExtension(destFile);

            for (int i = 1; i < int.MaxValue; i++)
            {
                var newName = Path.Combine(path, name + "_" + i + ext);
                if (!File.Exists(newName))
                    return newName;
            }
            throw new ApplicationException();
        }
    }
}
