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
            var project = projectRepo.GetProject(projectPath);
            model.Project = project;
            model.AvaliableCommands = captureCooridinator.GetAavaliableDeviceCommands(project);
            return View(model);
        }

        public IActionResult StartCapture(string projectPath, List<SelectedCommands> devices, bool svn, string template)
        {
            var project = projectRepo.GetProject(projectPath);
            var selectedDrives = devices.Where(d => d.Selected).Select(d => new DriveInfo(d.Drive));
            var commandId = captureCooridinator.StartCapture(project, template, selectedDrives, svn);
            return RedirectToAction(nameof(HomeController.ShowAmeTask), "Home", new { id = commandId });
        }

        public IActionResult ShowConflicts(string projectPath, string sourceDrive)
        {
            var project = projectRepo.GetProject(projectPath);
            var device = captureCooridinator.CreateCommand(new DriveInfo(sourceDrive), project.GetLocalPathRoot());
            return View(device);
        }
    }
}
