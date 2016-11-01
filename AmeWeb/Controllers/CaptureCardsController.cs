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
            model.AvaliableCommands = captureCooridinator.GetAavaliableCommands(project).Select(CreateViewModel).ToList();
            return View(model);
        }

        private TaskViewModel CreateViewModel(BackgroundTask cmd)
        {
            var vm = new TaskViewModel
            {
                State = cmd.State,
                Label = cmd.Label,
                CommandImagePath = $"images/{cmd.Label}.jpg",
                StateImagePath = $"images/task-state/{cmd.State}.gif",
            };
            FillViewModel(vm, cmd as DeviceMoveFileCommands);
            FillViewModel(vm, cmd as AddProjectToSvnCommand);
            return vm;
        }

        private void FillViewModel(TaskViewModel vm, DeviceMoveFileCommands cmd)
        {
            if (cmd == null)
                return;
            vm.Selected = cmd.FilesCount > 0;
            vm.SourceDrive = cmd.SourceDrive.Name;
            vm.HasWarning = cmd.GetAllConflictWithStoredFiles().Any();
            vm.Description = $"Pliki: {cmd.FilesCount}\r\nRozmiar: {cmd.FilesSize.DisplayFileSize()}";
            if (cmd.FilesCount > 0 && cmd.State == TaskState.Waiting)
                vm.ExecuteActionLink = Url.Action("StartCaptureSingleDevice", new { projectPath = cmd.DestinationRoot.FullName, sourceDrive = cmd.SourceDrive });
        }

        private void FillViewModel(TaskViewModel vm, AddProjectToSvnCommand cmd)
        {
            if (cmd == null)
                return;
            vm.Selected = true;
            vm.SourceDrive = "[internal]";
            vm.HasWarning = false;
            vm.Description = $"svn path: {cmd.SvnUri}";
            if (!string.IsNullOrEmpty(cmd.SvnUri) && cmd.State == TaskState.Waiting)
                vm.ExecuteActionLink = Url.Action("StartCaptureSingleDevice", new { projectPath = cmd.Project.LocalPathRoot });
        }

        public IActionResult StartCapture(string projectPath, List<SelectedDevices> devices)
        {
            var project = projectRepo.GetProject(projectPath);
            var selectedDrives = devices.Where(d => d.Selected).Select(d => new DriveInfo(d.Drive));
            var commandId = captureCooridinator.StartCapture(project, selectedDrives);
            return RedirectToAction(nameof(HomeController.ShowAmeTask), "Home", new { id = commandId });
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
