using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AmeCommon.Database
{
    public class AmeProjectRepository : IAmeProjectRepository
    {
        private readonly IOptions<AmeConfig> ameConfig;
        private readonly IDatabase database;
        private object sync = new object();
        public const string AmeProjectFileName = "ame-project.json";
        public AmeLocalSettings LocalSettings { get; set; }

        public AmeProjectRepository(IOptions<AmeConfig> ameConfig, IDatabase database)
        {
            this.ameConfig = ameConfig;
            this.database = database;
            LocalSettings = database.LocalSettings.FindById(1);
            if (LocalSettings == null)
            {
                LocalSettings = new AmeLocalSettings {SelectedProjectsRootPath = ameConfig.Value.ProjectDirectories.FirstOrDefault(), Id = 1};
                database.LocalSettings.Insert(LocalSettings);
            }
        }

        public AmeFotoVideoProject CreateProject(DateTime projectDate, string projectName)
        {
            var proj = new AmeFotoVideoProject
            {
                Id = Guid.NewGuid(),
                Name = projectName,
                EventDate = projectDate
            };
            proj.LocalPathRoot = Path.Combine(LocalSettings.SelectedProjectsRootPath, proj.UniqueName);
            Directory.CreateDirectory(proj.LocalPathRoot);
            SaveProject(proj);
            return proj;
        }

        public void ChangeRootDirectory()
        {
            var currentPath = LocalSettings.SelectedProjectsRootPath;
            var allPaths = ameConfig.Value.ProjectDirectories;
            var currentPos = ameConfig.Value.ProjectDirectories.IndexOf(currentPath);
            var nextPos = currentPos + 1;
            if (nextPos >= allPaths.Count)
                nextPos = 0;
            LocalSettings.SelectedProjectsRootPath = allPaths[nextPos];
            database.LocalSettings.Update(LocalSettings);
        }

        public List<AmeFotoVideoProject> AllProjects()
        {
            var rootDir = new DirectoryInfo(LocalSettings.SelectedProjectsRootPath);
            var projesct = rootDir.GetDirectories()
                .Where(d => d.GetFiles("ame-project.json").Any())
                .Select(f => GetProject(f.FullName))
                .Where(p => p != null)
                .ToList();
            return projesct;
        }

        public AmeFotoVideoProject GetProject(string projectPath)
        {
            try
            {
                var projectFilePath = Path.Combine(projectPath, "ame-project.json");
                var json = File.ReadAllText(projectFilePath);
                var project = JsonConvert.DeserializeObject<AmeFotoVideoProject>(json);
                project.LocalPathRoot = projectPath;
                return project;
            }
            catch (JsonSerializationException ex)
            {
                return null;
            }
        }

        public void SaveProject(AmeFotoVideoProject project)
        {
            lock (sync)
            {
                var projectFilePath = Path.Combine(project.LocalPathRoot, "ame-project.json");
                File.WriteAllText(projectFilePath, JsonConvert.SerializeObject(project, Formatting.Indented));
            }
        }


    }
}