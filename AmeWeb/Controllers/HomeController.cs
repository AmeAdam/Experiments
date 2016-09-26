using System;
using AmeCommon.CardsCapture;
using AmeWeb.Model;
using Microsoft.AspNetCore.Mvc;

namespace AmeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmeProjectRepository projectRepo;

        public HomeController(IAmeProjectRepository projectRepo)
        {
            this.projectRepo = projectRepo;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                KnownProjects = projectRepo.AllProjects(),
                DestiantionRoot = projectRepo.LocalSettings.SelectedProjectsRootPath
            };
            return View(model);
        }

        public IActionResult ShowAmeProject(int projectId)
        {
            var project = projectRepo.GetProject(projectId);
            return View(project);
        }

        public IActionResult RemoveProject(int projectId)
        {
            projectRepo.RemoveProject(projectId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        
        public IActionResult ChangeRootDirectory()
        {
            projectRepo.ChangeRootDirectory();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateProject(DateTime projectDate, string projectName)
        {
            var id = projectRepo.CreateProject(projectDate, projectName);
            return RedirectToAction(nameof(Index), "CaptureCards", new { projectId = id });
        }
    }
}
