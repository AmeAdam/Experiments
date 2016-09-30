using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public interface IDeviceCaptureFactory
    {
        IEnumerable<MoveFileCommand> Create(string sourceName, DriveInfo sourceDrive, DeviceCaptureInfo captureInfo, DirectoryInfo destinationDirectory);
    }

    public class DeviceCaptureFactory : IDeviceCaptureFactory
    {
        private IEnumerable<MoveFileCommand> CreateMoveDirectoryContentCommands(string sourceName, DriveInfo sourceDrive, DeviceCaptureInfo captureInfo, DirectoryInfo destinationDirectory)
        {
            var sourceDir = new DirectoryInfo(Path.Combine(sourceDrive.Name, captureInfo.SourceMask));
            return sourceDir.EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(sourceFile => new MoveFileCommand
                {
                    SourceDrive = sourceDrive,
                    SourceFile = sourceFile,
                    DestinationRoot = destinationDirectory,
                    File = new MediaFile
                    {
                        Size = sourceFile.Length,
                        Source = sourceName,
                        RelativePath = Path.Combine(captureInfo.TargetFolder, sourceFile.FullName.Substring(sourceDir.FullName.Length+1))
                    }
                });
        }

        private IEnumerable<MoveFileCommand> CreateMoveDirectoryContentFlatCommands(string sourceName, DriveInfo sourceDrive, DeviceCaptureInfo captureInfo, DirectoryInfo destinationDirectory)
        {
            var sourceDir = new DirectoryInfo(Path.Combine(sourceDrive.Name, captureInfo.SourceMask));
            return sourceDir.EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(sourceFile => new MoveFileCommand
                {
                    SourceDrive = sourceDrive,
                    SourceFile = sourceFile,
                    DestinationRoot = destinationDirectory,
                    File = new MediaFile
                    {
                        Size = sourceFile.Length,
                        Source = sourceName,
                        RelativePath = Path.Combine(captureInfo.TargetFolder, sourceFile.Name)
                    }
                });
        }

        private IEnumerable<MoveFileCommand> CreateMoveFilesCommands(string sourceName, DriveInfo sourceDrive, DeviceCaptureInfo captureInfo, DirectoryInfo destinationDirectory)
        {
            var masks = captureInfo.SourceMask.Split('\\');
            IEnumerable<DirectoryInfo> directories = new[] { new DirectoryInfo(sourceDrive.Name) };
            for (int i = 0; i < masks.Length - 1; i++)
            {
                var folderPattern = masks[i];
                directories = directories.SelectMany(d => d.GetDirectories(folderPattern));
            }
            var sourceFiles = directories.SelectMany(d => d.GetFiles(masks.Last()));
            return sourceFiles
                .Select(sourceFile => new MoveFileCommand
                {
                    SourceDrive = sourceDrive,
                    SourceFile = sourceFile,
                    DestinationRoot = destinationDirectory,
                    File = new MediaFile
                    {
                        Size = sourceFile.Length,
                        Source = sourceName,
                        RelativePath = Path.Combine(captureInfo.TargetFolder, sourceFile.Name)
                    }
                });

        }

        public IEnumerable<MoveFileCommand> Create(string sourceName, DriveInfo sourceDrive, DeviceCaptureInfo captureInfo, DirectoryInfo destinationDirectory)
        {
            switch (captureInfo.Command)
            {
                case "move-directory-content":
                    return CreateMoveDirectoryContentCommands(sourceName, sourceDrive, captureInfo, destinationDirectory);
                case "move-directory-content-flat":
                    return CreateMoveDirectoryContentFlatCommands(sourceName, sourceDrive, captureInfo, destinationDirectory);
                case "move-files":
                    return CreateMoveFilesCommands(sourceName, sourceDrive, captureInfo, destinationDirectory);
                default:
                    throw new ApplicationException("Not supported command type " + captureInfo.Command);
            }
        }
    }
}