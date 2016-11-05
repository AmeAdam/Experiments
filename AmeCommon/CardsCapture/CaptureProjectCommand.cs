using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AmeCommon.Model;
using AmeCommon.Tasks;
using Newtonsoft.Json;

namespace AmeCommon.CardsCapture
{
    public class CaptureProjectCommand : BackgroundTask
    {
        private readonly object saveSync = new object();
        public AmeFotoVideoProject Project { get; }
        public List<DeviceMoveFileCommands> DeviceCommands { get; set; }
        public AddResourcesToProjectCommand AddResourceCommand { get; set; }
        public AddProjectToSvnCommand CreateSvnCommand { get; set; }

        private readonly IAmeProjectRepository repository;
        private readonly DirectoryInfo destinationDirectory;
        public override string Name => "Tworzenie projektu - " + Project?.UniqueName;
        public override string Label => "AME-Project";


        public CaptureProjectCommand(IAmeProjectRepository repository, AmeFotoVideoProject project)
        {
            this.repository = repository;
            Project = project;
            destinationDirectory = new DirectoryInfo(Project.LocalPathRoot);
        }

        public override IEnumerable<BackgroundTask> ChildTasks
        {
            get
            {
                foreach (var cmd in DeviceCommands ?? new List<DeviceMoveFileCommands>())
                {
                    yield return cmd;
                }
                if (AddResourceCommand != null)
                    yield return AddResourceCommand;
                if (CreateSvnCommand != null)
                    yield return CreateSvnCommand;
            }
        }

        private Task ExecuteChildTask(DeviceMoveFileCommands cmd)
        {
            destinationDirectory.Create();

            var newMediaFiles = cmd.Commands.Select(c => c.File).ToList();
            var mediaFiles = new List<MediaFile>(Project.MediaFiles.Except(newMediaFiles));
            mediaFiles.AddRange(newMediaFiles);

            Project.MediaFiles = mediaFiles;
            SaveProject();
            cmd.OnComplete += DeviceCompleted;
            return cmd.ExecuteAsync();
        }

        private void DeviceCompleted(BackgroundTask task)
        {
            var command = (DeviceMoveFileCommands) task;
            SaveProject();
            command.DeleteCopiedFiles();
            command.Dispose();
        }

        protected override void Execute()
        {
            var copyTask = DeviceCommands.Select(ExecuteChildTask).ToArray();
            Task.WaitAll(copyTask);
            AddResourceCommand?.ExecuteAsync().Wait();
            CreateSvnCommand?.ExecuteAsync().Wait();
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
    }
}