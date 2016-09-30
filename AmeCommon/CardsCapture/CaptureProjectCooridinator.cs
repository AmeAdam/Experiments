using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeCommon.CardsCapture
{
    public class CaptureProjectCooridinator : ICaptureProjectCooridinator
    {
        private readonly IDeviceCaptureFactory factory;
        private readonly IDeviceRepository devices;
        private readonly IAmeProjectRepository repository;
        private readonly IDriveManager driveManager;
        private readonly ITasksManager taskManager;

        public CaptureProjectCooridinator(IDeviceCaptureFactory factory, IDeviceRepository devices, IAmeProjectRepository repository, IDriveManager driveManager, ITasksManager taskManager)
        {
            this.factory = factory;
            this.devices = devices;
            this.repository = repository;
            this.driveManager = driveManager;
            this.taskManager = taskManager;
        }

        public CaptureProjectCommand GetPendingCaptureProjectCommand()
        {
            return taskManager.GetPendingTasks().OfType<CaptureProjectCommand>().FirstOrDefault();
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
                if (pending.Project.Id != project.Id)
                    throw new ApplicationException("Nie można dodać zadania kopiowania dopuki poprzednie zadanie nie zostanie zakończone");
                if (pending.TryAppendTask(cmd))
                    return;
            }

            var newCommand = new CaptureProjectCommand(repository, project, new List<DeviceMoveFileCommands> {cmd});
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
            return new DeviceMoveFileCommands(driveManager)
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
