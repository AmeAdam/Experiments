﻿using System;
using AmeCommon.CardsCapture;
using AmeCommon.Tasks;
using AmeWeb.Model;
using Microsoft.AspNetCore.Mvc;

namespace AmeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmeProjectRepository projectRepo;
        private readonly ITasksManager tasksManager;

        public HomeController(IAmeProjectRepository projectRepo, ITasksManager tasksManager)
        {
            this.projectRepo = projectRepo;
            this.tasksManager = tasksManager;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Projects = projectRepo.AllProjects(),
                DestiantionRoot = projectRepo.LocalSettings.SelectedProjectsRootPath,
                Tasks = tasksManager.GetAllTasks(),
            };
            return View(model);
        }

        public IActionResult ShowAmeProject(string projectPath)
        {
            var project = projectRepo.GetProject(projectPath);
            return View(project);
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
            var proj = projectRepo.CreateProject(projectDate, projectName);
            return RedirectToAction(nameof(Index), "CaptureCards", new { projectPath = proj.LocalPathRoot });
        }

        public IActionResult ShowAmeTask(Guid id)
        {
            return View(tasksManager.GetTask(id));
        }
    }
}
