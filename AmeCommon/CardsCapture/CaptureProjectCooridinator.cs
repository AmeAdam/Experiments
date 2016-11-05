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

        public Guid StartCapture(AmeFotoVideoProject project, string template, IEnumerable<DriveInfo> drivesToCopy, bool addToSvn)
        {
            var copyCommands = drivesToCopy.Select(d => CreateCommand(d, project.GetLocalPathRoot())).ToList();

            var command = new CaptureProjectCommand(repository, project)
            {
                DeviceCommands = copyCommands,
                AddResourceCommand = new AddResourcesToProjectCommand(template, environment, project, driveManager),
                CreateSvnCommand = addToSvn ? new AddProjectToSvnCommand(config, project) : null
            };
            taskManager.StartTask(command);
            return command.Id;
        }

        public List<DeviceMoveFileCommands> GetAavaliableDeviceCommands(AmeFotoVideoProject project)
        {
            var res = driveManager
                .GetAvaliableRemovableDrives()
                .Select(d => CreateCommand(d, project.GetLocalPathRoot()))
                .Where(d => d != null)
                .ToList();
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
