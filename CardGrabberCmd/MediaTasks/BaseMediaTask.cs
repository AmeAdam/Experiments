using System.IO;
using CardGrabberCmd.MediaTasks.TaskHandlers;
using Microsoft.VisualBasic.FileIO;

namespace CardGrabberCmd.MediaTasks
{
    public abstract class BaseMediaTask : IMediaTask
    {
        public abstract void Execute();

        protected void MoveAllDirectoryContent(string absoluteSource, string absoluteTarget)
        {
            MoveAllFiles(absoluteSource, absoluteTarget);
            MoveAllChildDirectories(absoluteSource, absoluteTarget);
            Directory.Delete(absoluteSource);
        }

        protected void MoveAllChildDirectories(string absoluteSource, string absoluteTarget)
        {
            foreach (var childSourceDir in Directory.GetDirectories(absoluteSource))
            {
                var destDir = Path.Combine(absoluteTarget, Path.GetFileName(childSourceDir) ?? "");
                FileSystem.MoveDirectory(childSourceDir, destDir, UIOption.AllDialogs);
            }
        }

        protected void MoveAllFiles(string absoluteSource, string absoluteTarget, string searchPattern=null)
        {
            Directory.CreateDirectory(absoluteTarget);

            var sourceFiles = searchPattern != null
                ? Directory.GetFiles(absoluteSource, searchPattern)
                : Directory.GetFiles(absoluteSource);


            foreach (var sourceFile in sourceFiles)
            {
                var destFile = Path.Combine(absoluteTarget, Path.GetFileName(sourceFile) ?? "");
                File.Move(sourceFile, destFile);
            }
        }
    }
}