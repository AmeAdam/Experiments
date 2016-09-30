using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AmeCommon.Model;
using AmeCommon.Tasks;
using Newtonsoft.Json;

namespace AmeCommon.CardsCapture
{
    public class CaptureProjectCommand : BackgroundTask
    {
        private readonly object saveSync = new object();
        private readonly object taskSync = new object();
        public AmeFotoVideoProject Project { get; }
        public List<DeviceMoveFileCommands> DeviceCommands { get; private set; }
        public bool Completed => waitForComplete.WaitOne(0);
        private readonly IAmeProjectRepository repository;
        private readonly DirectoryInfo destinationDirectory;
        private readonly ManualResetEvent waitForComplete = new ManualResetEvent(false);

        public CaptureProjectCommand(IAmeProjectRepository repository, AmeFotoVideoProject project, List<DeviceMoveFileCommands> commands)
        {
            this.repository = repository;
            Project = project;
            destinationDirectory = new DirectoryInfo(Project.LocalPathRoot);
            DeviceCommands = commands;
        }

        public bool TryAppendTask(DeviceMoveFileCommands cmd)
        {
            lock (taskSync)
            {
                if (waitForComplete.WaitOne(0))
                    return false;
                DeviceCommands = new List<DeviceMoveFileCommands>(DeviceCommands) {cmd};
                ExecuteChildTask(cmd);
                return true;
            }
        }

        public List<DriveInfo> GetPendingDrives()
        {
            lock (taskSync)
            {
                return DeviceCommands.Where(cmd => !cmd.IsCompleted).Select(cmd => cmd.SourceDrive).ToList();
            }
        }

        private void ExecuteChildTask(DeviceMoveFileCommands cmd)
        {
            destinationDirectory.Create();
            cmd.SetDestinationRootPath(destinationDirectory);
            var mediaFiles = new List<MediaFile>(Project.MediaFiles);
            mediaFiles.AddRange(cmd.Commands.Select(c => c.File));
            Project.MediaFiles = mediaFiles;
            SaveProject();
            cmd.OnComplete += DeviceCompleted;
            cmd.ExecuteAsync();
        }

        private void DeviceCompleted(BackgroundTask task)
        {
            var command = (DeviceMoveFileCommands) task;
            SaveProject();
            command.DeleteCopiedFiles();

            lock (taskSync)
            {
                if (DeviceCommands.All(d => d.IsCompleted))
                    waitForComplete.Set();
            }
        }

        protected override void Execute()
        {
            foreach (var cmd in DeviceCommands)
                ExecuteChildTask(cmd);
            waitForComplete.WaitOne();
        }

        private void SaveProject()
        {
            lock (saveSync)
            {
                repository.SaveProject(Project);
                var projectFilePath = Path.Combine(Project.LocalPathRoot, "ame-project.json");
                File.WriteAllText(projectFilePath, JsonConvert.SerializeObject(Project, Formatting.Indented));
            }
        }

        public DeviceMoveFileCommands FindCommand(DriveInfo drive)
        {
            return DeviceCommands.FirstOrDefault(d => d.SourceDrive.Name.Equals(drive.Name));
        }

        public void AbortCapture(string uniqueName)
        {
            DeviceCommands.FirstOrDefault(cmd => cmd.Device.UniqueName == uniqueName)?.Cancel();
        }
    }
}