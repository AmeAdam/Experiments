using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AmeCommon.Database;
using AmeCommon.Model;
using AmeCommon.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AmeCommon.CardsCapture
{
    public class CaptureProjectCommand : BackgroundTask
    {
        private readonly object saveSync = new object();
        private readonly object taskSync = new object();
        public AmeFotoVideoProject Project { get; }
        public List<DeviceMoveFileCommands> DeviceCommands { get; private set; }
        public AddProjectToSvnCommand SvnCommand { get; private set; }
        public bool Completed => waitForComplete.WaitOne(0);
        private readonly IAmeProjectRepository repository;
        private readonly DirectoryInfo destinationDirectory;
        private readonly ManualResetEvent waitForComplete = new ManualResetEvent(false);
        public override string Name => "Tworzenie projektu - " + Project?.UniqueName;

        public CaptureProjectCommand(IOptions<AmeConfig> config, IAmeProjectRepository repository, AmeFotoVideoProject project, List<DeviceMoveFileCommands> commands)
        {
            this.repository = repository;
            Project = project;
            destinationDirectory = new DirectoryInfo(Project.LocalPathRoot);
            DeviceCommands = commands;
            SvnCommand = new AddProjectToSvnCommand(config, project);
        }

        public override IEnumerable<BackgroundTask> ChildTasks
        {
            get
            {
                foreach (var cmd in DeviceCommands)
                    yield return cmd;
                yield return SvnCommand;
            }
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

            var newMediaFiles = cmd.Commands.Select(c => c.File).ToList();
            var mediaFiles = new List<MediaFile>(Project.MediaFiles.Except(newMediaFiles));
            mediaFiles.AddRange(newMediaFiles);

            Project.MediaFiles = mediaFiles;
            SaveProject();
            cmd.OnComplete += DeviceCompleted;
            cmd.ExecuteAsync();
        }

        private void DeviceCompleted(BackgroundTask task)
        {
            try
            {
                var command = (DeviceMoveFileCommands) task;
                SaveProject();
                command.DeleteCopiedFiles();
                command.Dispose();
            }
            finally
            {
                lock (taskSync)
                {
                    if (DeviceCommands.All(d => d.IsCompleted))
                        waitForComplete.Set();
                }
            }
        }

        protected override void Execute()
        {
            foreach (var cmd in DeviceCommands)
                ExecuteChildTask(cmd);
            waitForComplete.WaitOne();
            SvnCommand.ExecuteAsync().Wait();
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