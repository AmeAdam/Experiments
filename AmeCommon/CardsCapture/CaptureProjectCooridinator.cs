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

        public CaptureProjectCooridinator(IHostingEnvironment environment, IOptions<AmeConfig> config, IDeviceCaptureFactory factory, IDeviceManager devices, IAmeProjectRepository repository, ITasksManager taskManager)
        {
            this.environment = environment;
            this.config = config;
            this.factory = factory;
            this.devices = devices;
            this.repository = repository;
            this.taskManager = taskManager;
        }

        public CaptureProjectCommand GetPendingCaptureProjectCommand()
        {
            return taskManager.GetPendingTasks().OfType<CaptureProjectCommand>().FirstOrDefault();
        }

        public CaptureProjectCommand CreateCaptureProjectCommand(AmeFotoVideoProject project, List<DeviceMoveFileCommands> commands)
        {
            commands.Add(new AddResourcesToProjectCommand(environment, project));
            return new CaptureProjectCommand(environment, config, repository, project, commands);
        }

        public List<DeviceMoveFileCommands> GetAllDevicesCommand(IEnumerable<DriveInfo> drives, DirectoryInfo destinationDirectory)
        {
            return drives.Select(d => GetDevicesCommand(d, destinationDirectory)).Where(d => d != null).ToList();
        }

        public DeviceMoveFileCommands GetDevicesCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory)
        {
            return CreateCommand(sourceDrive, destinationDirectory);
        }

        public void Execute(CaptureProjectCommand command)
        {
            if (GetPendingCaptureProjectCommand() != null)
                throw new ApplicationException("Poprzednia operacja nie została jeszcze zakończona!");
            taskManager.StartTask(command);
        }

        public void AppendCommand(AmeFotoVideoProject project, DeviceMoveFileCommands cmd)
        {
            var pending = GetPendingCaptureProjectCommand();
            if (pending != null)
            {
                if (pending.Project.LocalPathRoot != project.LocalPathRoot)
                    throw new ApplicationException("Nie można dodać zadania kopiowania dopuki poprzednie zadanie nie zostanie zakończone");
                if (pending.TryAppendTask(cmd))
                    return;
            }

            var newCommand = new CaptureProjectCommand(environment, config, repository, project, new List<DeviceMoveFileCommands> {cmd});
            taskManager.StartTask(newCommand);
        }

        public void AbortCapture(string uniqueName)
        {
            GetPendingCaptureProjectCommand()?.AbortCapture(uniqueName);
        }

        private DeviceMoveFileCommands CreateCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory)
        {
            var device = GetDevice(sourceDrive);
            if (device == null)
                return null;
            return new DeviceMoveFileCommands
            {
                SourceDrive = sourceDrive,
                Device = device,
                Commands = device.Captures.SelectMany(di => factory.Create(device.UniqueName, sourceDrive, di, destinationDirectory)).ToList()
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
