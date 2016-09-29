using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.Model;
using AmeCommon.Tasks;
using Newtonsoft.Json;

namespace AmeCommon.CardsCapture
{
    public class CaptureProjectCommand
    {
        private readonly object sync = new object();
        public AmeFotoVideoProject Project { get; set; }
        public List<DeviceMoveFileCommands> DeviceCommands { get; set; }
        public bool Completed => DeviceCommands.All(cmd => cmd.IsCompleted);
        private readonly IAmeProjectRepository repository;
        public DriveInfo DestinationDrive { get; set; }

        public CaptureProjectCommand(IAmeProjectRepository repository)
        {
            this.repository = repository;
        }

        public void SetDestinationRootPath(DirectoryInfo projectLocalRoot)
        {
            DestinationDrive = new DriveInfo(projectLocalRoot.Root.FullName);
            DeviceCommands.ForEach(cmd => cmd.SetDestinationRootPath(projectLocalRoot));
        }

        public void AppendTask(DeviceMoveFileCommands cmd)
        {
            DeviceCommands = new List<DeviceMoveFileCommands>(DeviceCommands){cmd};
            var mediaFiles = new List<MediaFile>(Project.MediaFiles);
            mediaFiles.AddRange(cmd.Commands.Select(c => c.File));
            Project.MediaFiles = mediaFiles;
            cmd.OnComplete += DeviceCompleted;
            cmd.ExecuteAsync();
        }

        private void DeviceCompleted(BackgroundTask task)
        {
            var command = (DeviceMoveFileCommands) task;
            SaveProject();
            command.DeleteCopiedFiles();
        }

        public void Execute()
        {
            Project.MediaFiles = DeviceCommands
                .SelectMany(d => d.Commands)
                .Select(cmd => cmd.File)
                .ToList();
            var destinatioDir = new DirectoryInfo(Project.LocalPathRoot);
            destinatioDir.Create();
            SaveProject();
            foreach (var cmd in DeviceCommands)
            {
                cmd.OnComplete += DeviceCompleted;
                cmd.SetDestinationRootPath(destinatioDir);
                cmd.ExecuteAsync();
            }
        }

        private void SaveProject()
        {
            lock (sync)
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