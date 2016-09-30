using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;
using AmeWeb.Model;
using Microsoft.AspNetCore.Mvc;

namespace AmeWeb.Controllers
{
    public class DevicesController : Controller
    {
        private IAmeProjectRepository repository;
        private readonly IDeviceRepository deviceRepository;
        private readonly ICaptureProjectCooridinator captureCooridinator;

        public DevicesController(IAmeProjectRepository repository, IDeviceRepository deviceRepository, ICaptureProjectCooridinator captureCooridinator)
        {
            this.repository = repository;
            this.deviceRepository = deviceRepository;
            this.captureCooridinator = captureCooridinator;
        }

        public IActionResult Index()
        {
            return View(ScanDevices());
        }

        private List<CardInfoViewModel> ScanDevices()
        {
            var model = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Removable)
                .Select(d => new CardInfoViewModel
                {
                    Device =
                        captureCooridinator.GetDevice(d) ??
                        new Device {Id = 0, UniqueName = "unknown", Captures = new List<DeviceCaptureInfo>()},
                    InsertedInDrive = d.Name
                })
                .ToList();

            var allStoredDevices = deviceRepository.GetAllDevices();
            foreach (var d in allStoredDevices)
            {
                if (model.All(modelDevice => modelDevice.Device.Id != d.Id))
                    model.Add(new CardInfoViewModel {Device = d});
            }
            return model;
        }

        public IActionResult DeviceEdit(int deviceId, string drive)
        {
            var device = string.IsNullOrEmpty(drive) ? 
                deviceRepository.GetDevice(deviceId) :
                captureCooridinator.GetDevice(new DriveInfo(drive));
            var model = new CardInfoViewModel
            {
                Device = device ?? new Device { Id = 0, UniqueName = "unknown", Captures = new List<DeviceCaptureInfo>() },
                InsertedInDrive = drive,
            };
            return View(model);
        }

        public IActionResult AssignDevice(int deviceId, string drive)
        {
            var device = deviceRepository.GetDevice(deviceId);
                var doc = new XDocument(
                    new XElement(XName.Get("ame-card", "http://kamerzysta.bydgoszcz.pl"),
                        new XElement(XName.Get("id", "http://kamerzysta.bydgoszcz.pl"), device.UniqueName)
                    ));
                doc.Save(Path.Combine(drive, "ame.xml"));
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteDeviceAssociation(string drive)
        {
            System.IO.File.Delete(Path.Combine(drive, "ame.xml"));
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SaveDevice(CardInfoViewModel model)
        {
            var device = model.Device;
            device.Captures.RemoveAll(cmd => string.IsNullOrEmpty(cmd.SourceMask));
            deviceRepository.UpdateDevice(device);
            if (!string.IsNullOrEmpty(model.InsertedInDrive))
            {
                var doc = new XDocument(
                    new XElement(XName.Get("ame-card", "http://kamerzysta.bydgoszcz.pl"),
                        new XElement(XName.Get("id", "http://kamerzysta.bydgoszcz.pl"), model.Device.UniqueName)
                    ));
                doc.Save(Path.Combine(model.InsertedInDrive, "ame.xml"));
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteDevice(int id)
        {
            deviceRepository.RemoveDevice(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
