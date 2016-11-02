using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AmeCommon.Database;
using AmeCommon.Model;
using AmeCommon.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace AmeCommon.CardsCapture
{
    public class CaptureProjectCooridinator : ICaptureProjectCooridinator
    {
        private readonly IHostingEnvironment environment;
        private readonly IOptions<AmeConfig> config;
        private readonly IDeviceCaptureFactory factory;
        private readonly IDeviceManager devices;
        private readonly IAmeProjectRepository repository;
        private readonly ITasksManager taskManager;
        private readonly IDriveManager driveManager;

        public CaptureProjectCooridinator(IHostingEnvironment environment, IOptions<AmeConfig> config, IDeviceCaptureFactory factory, IDeviceManager devices, IAmeProjectRepository repository, ITasksManager taskManager, IDriveManager driveManager)
        {
            this.environment = environment;
            this.config = config;
            this.factory = factory;
            this.devices = devices;
            this.repository = repository;
            this.taskManager = taskManager;
            this.driveManager = driveManager;
        }

//        public CaptureProjectCommand CreateCaptureProjectCommand(AmeFotoVideoProject project, List<DeviceMoveFileCommands> commands)
//        {
//            commands.Add(new AddResourcesToProjectCommand(environment, project));
//            return new CaptureProjectCommand(environment, config, repository, project, commands, driveManager);
//        }

        public Guid StartCapture(AmeFotoVideoProject project, List<BackgroundTask> childCommands)
        {
            if (childCommands.Any())
            {
                var command = new CaptureProjectCommand(repository, project, childCommands);
                taskManager.StartTask(command);
                return command.Id;
            }
            return Guid.Empty;
        }

        public List<BackgroundTask> GetAavaliableCommands(AmeFotoVideoProject project)
        {
            var res = driveManager
                .GetAvaliableRemovableDrives()
                .Select(d => CreateCommand(d, project.GetLocalPathRoot()))
                .Where(d => d != null)
                .Cast<BackgroundTask>()
                .ToList();
            res.Add(new AddResourcesToProjectCommand("Wesele", environment, project.GetLocalPathRoot(), driveManager));
            res.Add(new AddResourcesToProjectCommand("Przedszkole", environment, project.GetLocalPathRoot(), driveManager));
            res.Add(new AddProjectToSvnCommand(config, project));
            return res;
        }

        public DeviceMoveFileCommands CreateCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory)
        {
            var device = GetDevice(sourceDrive);
            if (device == null)
                return null;
            return new DeviceMoveFileCommands(driveManager)
            {
                SourceDrive = sourceDrive,
                Device = device,
                DestinationRoot = destinationDirectory,
                Commands = device.Captures.SelectMany(di => factory.Create(device.UniqueName, sourceDrive, di, destinationDirectory)).ToList(),
            };
        }

        public BackgroundTask CreateCommand(string commandType, string commandParam, AmeFotoVideoProject project)
        {
            switch (commandType)
            {
                case nameof(DeviceMoveFileCommands):
                    return CreateCommand(new DriveInfo(commandParam), project.GetLocalPathRoot());
                case nameof(AddResourcesToProjectCommand):
                    return new AddResourcesToProjectCommand(commandParam, environment, project.GetLocalPathRoot(), driveManager);
                case nameof(AddProjectToSvnCommand):
                    return new AddProjectToSvnCommand(config, project);
                default:
                    return null;
            }
        }

        public Device GetDevice(DriveInfo sourceDrive)
        {
            var ameCard = ReadCardSettings(sourceDrive);
            if (ameCard == null)
                return null;
            return devices.GetDevice(ameCard.Id);
        }

        private static AmeCardSettings ReadCardSettings(DriveInfo drive)
        {
            var ameFilePath = Path.Combine(drive.Name, "ame.xml");
            if (!File.Exists(ameFilePath))
                return null;
            try
            {
                var doc = XDocument.Load(ameFilePath);
                XmlSerializer xs = new XmlSerializer(typeof(AmeCardSettings));
                using (var xr = doc.CreateReader())
                {
                    return (AmeCardSettings)xs.Deserialize(xr);
                }
            }
            catch (XmlException)
            {
                return null;
            }
        }
    }
}
