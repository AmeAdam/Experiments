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
        private readonly ICaptureProjectCooridinator captureCooridinator;
        private readonly IAmeProjectRepository projectRepo;

        public CaptureCardsController(ICaptureProjectCooridinator captureCooridinator, IAmeProjectRepository projectRepo)
        {
            this.captureCooridinator = captureCooridinator;
            this.projectRepo = projectRepo;
        }

        public IActionResult Index(string projectPath)
        {
            var model = new CaptureCardsViewModel();
            var pendingTask = captureCooridinator.GetPendingCaptureProjectCommand();
            if (pendingTask != null)
            {
                model.PendingTask = pendingTask;
                model.Project = pendingTask.Project;

                var avaliableDrives = GetAvaliableDrives(pendingTask);
                var avaliableToCapture = captureCooridinator.GetAllDevicesCommand(avaliableDrives, model.Project.GetLocalPathRoot());
                avaliableToCapture.RemoveAll(d => d.Commands.Count == 0 && !pendingTask.DeviceCommands.Contains(d));
                model.AvaliableCommands = new List<DeviceMoveFileCommands>(model.PendingTask.DeviceCommands);
                model.AvaliableCommands.AddRange(avaliableToCapture);
            }
            else
            {
                model.Project = projectRepo.GetProject(projectPath);
                model.AvaliableCommands = captureCooridinator.GetAllDevicesCommand(DriveInfo.GetDrives(), model.Project.GetLocalPathRoot());
            }
            return View(model);
        }

        private List<DriveInfo> GetAvaliableDrives(CaptureProjectCommand pendingTask)
        {
            return DriveInfo.GetDrives()
                .Select(d => d.Name)
                .Except(pendingTask.GetPendingDrives().Select(d => d.Name))
                .Select(n => new DriveInfo(n))
                .ToList();
        }

        public IActionResult StartCapture(string projectPath, List<SelectedDevices> devices)
        {
            var project = projectRepo.GetProject(projectPath);
            var selectedDevices = captureCooridinator.GetAllDevicesCommand(
                devices.Where(d => d.Selected).Select(d => new DriveInfo(d.Drive)), project.GetLocalPathRoot()).ToList();

            if (selectedDevices.Any())
            {
                var cmd = new CaptureProjectCommand(projectRepo, project, selectedDevices);
                captureCooridinator.Execute(cmd);
            }
            return RedirectToAction(nameof(Index), new { projectPath });
        }

        public IActionResult StartCaptureSingleDevice(string projectPath, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectPath);
            var command = captureCooridinator.GetDevicesCommand(new DriveInfo(sourceDrive), project.GetLocalPathRoot());
            captureCooridinator.AppendCommand(project, command);
            return RedirectToAction(nameof(Index), new { projectPath });
        }

        public IActionResult AbortCaptureDevice(int projectId, string deviceName)
        {
            captureCooridinator.AbortCapture(deviceName);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        public IActionResult ShowConflicts(string projectPath, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectPath);
            var device = captureCooridinator.GetDevicesCommand(new DriveInfo(sourceDrive), project.GetLocalPathRoot());
            var destinationDir = new DirectoryInfo(project.LocalPathRoot);
            device.SetDestinationRootPath(destinationDir);
            return View(device);
        }
    }
}
