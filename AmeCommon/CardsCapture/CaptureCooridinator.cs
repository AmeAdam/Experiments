using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public class CaptureCooridinator : ICaptureCooridinator
    {
        private CaptureProjectCommand currentWork;
        private readonly IDeviceCaptureFactory factory;
        private readonly IDeviceRepository devices;
        private readonly IAmeProjectRepository repository;
        private readonly IDriveManager driveManager;

        public CaptureCooridinator(IDeviceCaptureFactory factory, IDeviceRepository devices, IAmeProjectRepository repository, IDriveManager driveManager)
        {
            this.factory = factory;
            this.devices = devices;
            this.repository = repository;
            this.driveManager = driveManager;
        }

        public IEnumerable<DeviceMoveFileCommands> GetAllDevicesCommand(IEnumerable<DriveInfo> drives)
        {
            return drives.Select(GetDevicesCommand).Where(d => d != null);
        }

        public DeviceMoveFileCommands GetDevicesCommand(DriveInfo sourceDrive)
        {
            var pending = currentWork?.FindCommand(sourceDrive);
            return pending ?? CreateCommand(sourceDrive);
        }

        public void Execute(CaptureProjectCommand command)
        {
            if (currentWork != null && !currentWork.Completed)
                throw new ApplicationException("Poprzednia operacja nie została jeszcze zakończona!");

            currentWork = command;
            currentWork.Execute();
        }

        public void AppendCommand(AmeFotoVideoProject project, DeviceMoveFileCommands cmd)
        {
            if (currentWork == null)
            {
                currentWork = new CaptureProjectCommand(repository)
                {
                    Project = project,
                    DeviceCommands = new List<DeviceMoveFileCommands> {cmd}
                };
                currentWork.Execute();
            }
            else if (currentWork.Project.UniqueName == project.UniqueName)
            {
                currentWork.AppendTask(cmd);
            }
            else
            {
                throw new ApplicationException("Nie można dodać zadania kopiowania, inne zadanie nie zostało jeszcze zakończone.");
            }
        }

        public void AbortCapture(string uniqueName)
        {
            currentWork.AbortCapture(uniqueName);
        }

        private DeviceMoveFileCommands CreateCommand(DriveInfo sourceDrive)
        {
            var device = GetDevice(sourceDrive);
            if (device == null)
                return null;
            return new DeviceMoveFileCommands(driveManager)
            {
                SourceDrive = sourceDrive,
                Device = device,
                Commands = device.Captures.SelectMany(di => factory.Create(device.UniqueName, sourceDrive, di)).ToList()
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
