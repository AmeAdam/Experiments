using System.IO;
using AmeCommon.MediaTasks.TaskHandlers;
using Microsoft.VisualBasic.FileIO;
using System;

namespace AmeCommon.MediaTasks
{
    public abstract class BaseMediaTask : IMediaTask
    {
        public abstract void Execute();

        protected void MoveAllDirectoryContent(string absoluteSource, string absoluteTarget, bool deleteSourcDir)
        {
            if (!Directory.Exists(absoluteSource))
                return;
            MoveAllFiles(absoluteSource, absoluteTarget);
            MoveAllChildDirectories(absoluteSource, absoluteTarget);
            if (deleteSourcDir)
                Directory.Delete(absoluteSource);
        }

        protected void MoveAllChildDirectories(string absoluteSource, string absoluteTarget)
        {
            if (!Directory.Exists(absoluteSource))
                return;

            foreach (var childSourceDir in Directory.GetDirectories(absoluteSource))
            {
                var destDir = Path.Combine(absoluteTarget, Path.GetFileName(childSourceDir) ?? "");
                FileSystem.MoveDirectory(childSourceDir, destDir, UIOption.AllDialogs);
            }
        }

        protected void MoveAllFiles(string absoluteSource, string absoluteTarget, string searchPattern=null)
        {
            if (!Directory.Exists(absoluteSource))
                return;

            Directory.CreateDirectory(absoluteTarget);

            var sourceFiles = searchPattern != null
                ? Directory.GetFiles(absoluteSource, searchPattern)
                : Directory.GetFiles(absoluteSource);


            foreach (var sourceFile in sourceFiles)
            {
                var destFile = Path.Combine(absoluteTarget, Path.GetFileName(sourceFile) ?? "");
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

        private void MoveFile(string sourceFile, string destFile)
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

            for(int i=1; i<int.MaxValue; i++)
            {
                var newName = Path.Combine(path, name + "_" + i + ext);
                if (!File.Exists(newName))
                    return newName;
            }
            throw new ApplicationException();
        }
    }
}