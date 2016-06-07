using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using AmeCommon.Settings;

namespace AmeCommon.MediaTasks.MoveFilesCommands
{
    public class MoveFile
    {
        private readonly string sourceFileAbsolutePath;
        private readonly DestinationDirectory targetProject;
        private string targetFileAbsolutePath;
        private readonly AmeProjectFile ameProjectFile;
        private const int FileBufferSize = 8*1024*1024;

        public MoveFile(string sourceFileAbsolutePath, DestinationDirectory targetProject, string targetFileRelativePath)
        {
            this.sourceFileAbsolutePath = sourceFileAbsolutePath;
            this.targetProject = targetProject;
            targetFileAbsolutePath = targetProject.GetAbsolutePath(targetFileRelativePath);
            ameProjectFile = new AmeProjectFile
            {
                RelativePath = targetFileRelativePath,
                OriginalName = Path.GetFileName(sourceFileAbsolutePath)
            };
        }

        public AmeProjectFile Execute()
        {
            var srcFile = new FileInfo(sourceFileAbsolutePath);
            var dstFile = new FileInfo(targetFileAbsolutePath);
            ameProjectFile.Size = srcFile.Length;

            if (dstFile.Exists)
            {
                if (AreFilesSameByContent(srcFile, dstFile))
                    File.Delete(sourceFileAbsolutePath);
                else
                {
                    targetFileAbsolutePath = GetAlternativePath(targetFileAbsolutePath);
                    ameProjectFile.RelativePath = targetProject.GetRelativePath(targetFileAbsolutePath);
                    File.Move(sourceFileAbsolutePath, targetFileAbsolutePath);
                }
            }
            else
                File.Move(sourceFileAbsolutePath, targetFileAbsolutePath);

            ameProjectFile.Md5 = CalculateMd5Hash(dstFile);
            return ameProjectFile;
        }

        private string CalculateMd5Hash(FileInfo file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = file.OpenRead())
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
        }

        private static string GetAlternativePath(string destFile)
        {
            var ext = Path.GetExtension(destFile);
            var name = Path.GetFileNameWithoutExtension(destFile);
            var dir = Path.GetDirectoryName(destFile);
            if (dir == null)
                throw new ApplicationException("incorrect target file path: " + destFile);

            for (int i = 1; i < int.MaxValue; i++)
            {
                var newName = Path.Combine(dir, name + "_" + i + ext);
                if (!File.Exists(newName))
                    return newName;
            }
            throw new ApplicationException("Unable to find alternative name for file " + destFile);
        }

        private bool AreFilesSameByContent(FileInfo srcFile, FileInfo dstFile)
        {
            if (srcFile.Length != dstFile.Length)
                return false;

            using (var src = srcFile.OpenRead())
            {
                using (var dst = dstFile.OpenRead())
                {
                    var srcBuff = new byte[FileBufferSize];
                    var dstBuff = new byte[FileBufferSize];

                    while (true)
                    {
                        var srcRead = ReadFromStream(srcBuff, src);
                        if (srcRead == 0)
                            break;
                        ReadFromStream(dstBuff, dst);
                        if (memcmp(srcBuff, dstBuff, srcRead) != 0)
                            return false;
                    }
                    return true;
                }
            }
        }

        private int ReadFromStream(byte[] buffer, FileStream str)
        {
            int bytesToRead = buffer.Length;
            int pos = 0;

            while (bytesToRead > 0)
            {
                var readedBytes = str.Read(buffer, pos, bytesToRead);
                if (readedBytes == 0)
                    break;
                pos += readedBytes;
                bytesToRead -= readedBytes;
            }
            return pos;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
    }
}