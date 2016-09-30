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

        public IActionResult Index(int projectId)
        {
            var model = new CaptureCardsViewModel();
            var pendingTask = captureCooridinator.GetPendingCaptureProjectCommand();
            if (pendingTask != null)
            {
                model.PendingTask = pendingTask;
                model.Project = pendingTask.Project;
                var avaliableDrives = GetAvaliableDrives(pendingTask);
                model.AvaliableCommands = new List<DeviceMoveFileCommands>(model.PendingTask.DeviceCommands);
                model.AvaliableCommands.AddRange(captureCooridinator.GetAllDevicesCommand(avaliableDrives, model.Project.GetLocalPathRoot()));
            }
            else
            {
                model.Project = projectRepo.GetProject(projectId);
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

        public IActionResult StartCapture(int projectId, List<SelectedDevices> devices)
        {
            var project = projectRepo.GetProject(projectId);
            var selectedDevices = captureCooridinator.GetAllDevicesCommand(
                devices.Where(d => d.Selected).Select(d => new DriveInfo(d.Drive)), project.GetLocalPathRoot()).ToList();

            var cmd = new CaptureProjectCommand(projectRepo, project, selectedDevices);
            captureCooridinator.Execute(cmd);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        public IActionResult StartCaptureSingleDevice(int projectId, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectId);
            var command = captureCooridinator.GetDevicesCommand(new DriveInfo(sourceDrive), project.GetLocalPathRoot());
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
            var device = captureCooridinator.GetDevicesCommand(new DriveInfo(sourceDrive), project.GetLocalPathRoot());
            var destinationDir = new DirectoryInfo(project.LocalPathRoot);
            device.SetDestinationRootPath(destinationDir);
            return View(device);
        }
    }
}
