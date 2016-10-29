using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Tasks;
using AmeWeb.Model;
using Microsoft.AspNetCore.Mvc;

namespace AmeWeb.Controllers
{
    public class CaptureCardsController : Controller
    {
        private readonly ICaptureProjectCooridinator captureCooridinator;
        private readonly IAmeProjectRepository projectRepo;
        private readonly ITasksManager taskManager;

        public CaptureCardsController(ICaptureProjectCooridinator captureCooridinator, IAmeProjectRepository projectRepo, ITasksManager taskManager)
        {
            this.captureCooridinator = captureCooridinator;
            this.projectRepo = projectRepo;
            this.taskManager = taskManager;
        }

        public IActionResult Index(string projectPath)
        {
            var model = new CaptureCardsViewModel();
            model.Project = projectRepo.GetProject(projectPath);
            model.AvaliableCommands = captureCooridinator.GetAavaliableDevicesCommand(model.Project.GetLocalPathRoot());
            return View(model);
        }

        public IActionResult StartCapture(string projectPath, List<SelectedDevices> devices)
        {
            var project = projectRepo.GetProject(projectPath);
            var selectedDrives = devices.Where(d => d.Selected).Select(d => new DriveInfo(d.Drive));
            var commandId = captureCooridinator.StartCapture(project, selectedDrives);
            return RedirectToAction(nameof(ViewCaptureCommand), new { commandId });
        }

        public IActionResult ViewCaptureCommand(Guid commandId)
        {
            var task = (CaptureProjectCommand)taskManager.GetTask(commandId);
            return View(task);
        }

        public IActionResult StartCaptureSingleDevice(string projectPath, string sourceDrive)
        {
            return StartCapture(projectPath, new List<SelectedDevices>
            {
                new SelectedDevices
                {
                    Selected = true,
                    Drive = sourceDrive
                }
            });
        }

        public IActionResult AbortCaptureDevice(int projectId, string deviceName)
        {
            //captureCooridinator.AbortCapture(deviceName);
            return RedirectToAction(nameof(Index), new { projectId });
        }

        public IActionResult ShowConflicts(string projectPath, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectPath);
            var device = captureCooridinator.CreateCommand(new DriveInfo(sourceDrive), project.GetLocalPathRoot());
            var destinationDir = new DirectoryInfo(project.LocalPathRoot);
            device.SetDestinationRootPath(destinationDir);
            return View(device);
        }
    }
}
