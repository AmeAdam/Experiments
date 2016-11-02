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
        public List<BackgroundTask> ChildCommands { get; }
//        public AddProjectToSvnCommand SvnCommand { get; private set; }
        public bool Completed => waitForComplete.WaitOne(0);
        private readonly IAmeProjectRepository repository;
        private readonly DirectoryInfo destinationDirectory;
        private readonly ManualResetEvent waitForComplete = new ManualResetEvent(false);
        public override string Name => "Tworzenie projektu - " + Project?.UniqueName;
        public override string Label => "AME-Project";


        public CaptureProjectCommand(IAmeProjectRepository repository, AmeFotoVideoProject project, List<BackgroundTask> commands)
        {
            this.repository = repository;
            Project = project;
            destinationDirectory = new DirectoryInfo(Project.LocalPathRoot);
            ChildCommands = commands;
//            SvnCommand = new AddProjectToSvnCommand(config, project);
        }

        public override IEnumerable<BackgroundTask> ChildTasks => ChildCommands;

        private void ExecuteChildTask(DeviceMoveFileCommands cmd)
        {
            destinationDirectory.Create();

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
                    if (ChildCommands.OfType<DeviceMoveFileCommands>().All(d => d.IsCompleted))
                        waitForComplete.Set();
                }
            }
        }

        protected override void Execute()
        {
            foreach (var cmd in ChildCommands.OfType<DeviceMoveFileCommands>())
                ExecuteChildTask(cmd);
            waitForComplete.WaitOne();
            foreach (var cmd in ChildCommands.OfType<AddProjectToSvnCommand>())
                cmd.ExecuteAsync().Wait();
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