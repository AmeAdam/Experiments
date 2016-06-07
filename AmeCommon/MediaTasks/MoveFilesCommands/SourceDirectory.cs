using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AmeCommon.MediaTasks.MoveFilesCommands
{
    public class SourceDirectory
    {
        private readonly string sourceDirectory;
        private readonly string rootPath;

        public SourceDirectory(string sourceDirectory)
        {
            this.sourceDirectory = sourceDirectory;
            rootPath = (Path.GetPathRoot(sourceDirectory) ?? "").ToLower(CultureInfo.InvariantCulture);
        }

        public string Name => Path.GetFileName(sourceDirectory);

        public static SourceDirectory Create(string sourceDirectory)
        {
            return new SourceDirectory(sourceDirectory);
        }

        public IEnumerable<SourceDirectory> Childs => Directory.GetDirectories(sourceDirectory).Select(Create);

        public bool TryDelete()
        {
            try
            {
                Directory.Delete(sourceDirectory, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<string> GetFiles(string patter = null)
        {
            if (patter == null)
                return Directory.GetFiles(sourceDirectory);
            return Directory.GetFiles(sourceDirectory, patter);
        }

        public static IEnumerable<string> GetFilesList(string sourcePathPattern)
        {
            var splittedPattern = sourcePathPattern.Split('\\', '/');
            if (splittedPattern.Length < 2)
                throw new ApplicationException("Incorrect file pattern " + sourcePathPattern);

            var filePattern = splittedPattern.Last();
            var sourceDirectories = new List<string> { splittedPattern[0] + "\\" };
            for (int i = 1; i < splittedPattern.Length - 1; i++)
                sourceDirectories = GetMatchedDirectories(sourceDirectories, splittedPattern[i]);
            return sourceDirectories.SelectMany(d => Directory.GetFiles(d, filePattern));
        }

        private static List<string> GetMatchedDirectories(List<string> sourceDirectories, string pattern)
        {
            var result = new List<string>();
            foreach (var dir in sourceDirectories)
                result.AddRange(Directory.GetDirectories(dir, pattern));
            return result;
        }

        public FileStream GetReadFileStream(string relativeSourcePath)
        {
            return File.OpenRead(GetAbsolutePath(relativeSourcePath));
        }

        public string GetSourceDrive()
        {
            return rootPath;
        }

        private string GetAbsolutePath(string relativeSourcePath)
        {
            return Path.Combine(sourceDirectory, relativeSourcePath);
        }

        public void DeleteFile(string relativeSourcePath)
        {
            File.Delete(GetAbsolutePath(relativeSourcePath));
        }
    }
}