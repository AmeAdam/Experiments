using System.Collections.Generic;
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
                CommandImagePath = $"images/{cmd.Label}.jpg",
                Label = cmd.Label
            };
            FillViewModel(vm, cmd as DeviceMoveFileCommands);
            FillViewModel(vm, cmd as AddResourcesToProjectCommand);
            FillViewModel(vm, cmd as AddProjectToSvnCommand);
            return vm;
        }

        private void FillViewModel(TaskViewModel vm, DeviceMoveFileCommands cmd)
        {
            if (cmd == null)
                return;
            vm.Selected = cmd.FilesCount > 0;
            vm.CommandType = nameof(DeviceMoveFileCommands);
            vm.Param = cmd.SourceDrive.Name;
            vm.HasWarning = cmd.GetAllConflictWithStoredFiles().Any();
            vm.Description = $"Pliki: {cmd.FilesCount}\r\nRozmiar: {cmd.FilesSize.DisplayFileSize()}";
        }

        private void FillViewModel(TaskViewModel vm, AddResourcesToProjectCommand cmd)
        {
            if (cmd == null)
                return;
            vm.Selected = false;
            vm.CommandType = nameof(AddResourcesToProjectCommand);
            vm.Param = cmd.Mp3FolderName;
            vm.HasWarning = cmd.GetAllConflictWithStoredFiles().Any();
            vm.Description = $"szablon projektu {cmd.Mp3FolderName}";
            vm.CommandImagePath = "images/resources.jpg";
        }

        private void FillViewModel(TaskViewModel vm, AddProjectToSvnCommand cmd)
        {
            if (cmd == null)
                return;
            vm.Selected = true;
            vm.CommandType = nameof(AddProjectToSvnCommand);
            vm.Param = "svn";
            vm.HasWarning = false;
            vm.Description = $"svn path: {cmd.SvnUri}";
        }

        public IActionResult StartCapture(string projectPath, List<SelectedCommands> devices)
        {
            var project = projectRepo.GetProject(projectPath);
            var selectedCommands = devices
                .Where(d => d.Selected).Select(d => captureCooridinator.CreateCommand(d.Task, d.Param, project))
                .ToList();
            var commandId = captureCooridinator.StartCapture(project, selectedCommands);
            return RedirectToAction(nameof(HomeController.ShowAmeTask), "Home", new { id = commandId });
        }

        public IActionResult ShowConflicts(string projectPath, string commandType, string param)
        {
            var project = projectRepo.GetProject(projectPath);
            var device = captureCooridinator.CreateCommand(commandType, param, project) as DeviceMoveFileCommands;
            return View(device);
        }
    }
}
