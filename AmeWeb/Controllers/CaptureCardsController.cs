using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeWeb.Model;
using Microsoft.AspNetCore.Mvc;

namespace AmeWeb.Controllers
{
    public class CaptureCardsController : Controller
    {
        private readonly ICaptureCooridinator captureCooridinator;
        private readonly IAmeProjectRepository projectRepo;

        public CaptureCardsController(ICaptureCooridinator captureCooridinator, IAmeProjectRepository projectRepo)
        {
            this.captureCooridinator = captureCooridinator;
            this.projectRepo = projectRepo;
        }

        public IActionResult Index(int projectId)
        {
            var project = projectRepo.GetProject(projectId);
            var devices = captureCooridinator.GetAllDevicesCommand(DriveInfo.GetDrives()).ToList();
            var destinationDir = new DirectoryInfo(project.LocalPathRoot);

            var model = new CaptureProjectCommand(projectRepo)
            {
                DeviceCommands = devices,
                Project = project
            };
            model.SetDestinationRootPath(destinationDir);
            return View(model);
        }

        public IActionResult StartCapture(int projectId, List<SelectedDevices> devices)
        {
            var cmd = new CaptureProjectCommand(projectRepo)
            {
                Project = projectRepo.GetProject(projectId),
                DeviceCommands = captureCooridinator.GetAllDevicesCommand(
                    devices.Where(d => d.Selected).Select(d => new DriveInfo(d.Drive))).ToList()
            };
            captureCooridinator.Execute(cmd);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        public IActionResult StartCaptureSingleDevice(int projectId, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectId);
            var command = captureCooridinator.GetDevicesCommand(new DriveInfo(sourceDrive));
            captureCooridinator.AppendCommand(project, command);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        public IActionResult AbortCaptureDevice(int projectId, string deviceName)
        {
            captureCooridinator.AbortCapture(deviceName);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        public IActionResult ShowConflicts(int projectId, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectId);
            var device = captureCooridinator.GetDevicesCommand(new DriveInfo(sourceDrive));
            var destinationDir = new DirectoryInfo(project.LocalPathRoot);
            device.SetDestinationRootPath(destinationDir);
            return View(device);
        }
    }
}
