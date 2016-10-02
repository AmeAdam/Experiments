using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AmeCommon.Database
{
    public class Repository : IDeviceRepository, IAmeProjectRepository, IDisposable
    {
        private readonly IHostingEnvironment environment;
        private object sync = new object();
        public const string AmeProjectFileName = "ame-project.json";
        private LiteDatabase db;
        private readonly LiteCollection<AmeFotoVideoProject> projects;
        private readonly LiteCollection<Device> devices;
        private readonly LiteCollection<AmeLocalSettings> localSettings;
        public AmeLocalSettings LocalSettings { get; set; }

        public Repository(IOptions<AmeConfig> ameConfig, IHostingEnvironment environment)
        {
            this.environment = environment;
            db = new LiteDatabase(ameConfig.Value.LiteDbDatabaseFilePath);

            var isDbEmpty = !db.GetCollectionNames().Any();

            devices = db.GetCollection<Device>("devices");
            projects = db.GetCollection<AmeFotoVideoProject>("projects");
            localSettings = db.GetCollection<AmeLocalSettings>("localSettings");

            if (isDbEmpty)
                InitializeDatabase();

            LocalSettings = localSettings.FindById(1);
            if (LocalSettings == null)
            {
                LocalSettings = new AmeLocalSettings {SelectedProjectsRootPath = ameConfig.Value.DefaultProjectPath, Id = 1};
                localSettings.Insert(LocalSettings);
            }
        }

        private void InitializeDatabase()
        {
            var devicesFilePath = Path.Combine(environment.WebRootPath, "initial-database", "devices.json");
            devices.Insert(JsonConvert.DeserializeObject<List<Device>>(File.ReadAllText(devicesFilePath)));
        }

        public Device GetDevice(string name)
        {
            return devices.FindOne(d => d.UniqueName == name);
        }

        public Device GetDevice(int id)
        {
            return devices.FindById(id);
        }

        public List<Device> GetAllDevices()
        {
            return devices.FindAll().ToList();
        }

        public void UpdateDevice(Device device)
        {
            if (device.Id == 0)
                devices.Insert(device);
            else
                devices.Update(device);
        }

        public void RemoveDevice(int id)
        {
            devices.Delete(id);
        }

        public AmeFotoVideoProject CreateProject(DateTime projectDate, string projectName)
        {
            var proj = new AmeFotoVideoProject
            {
                //Id = Guid.NewGuid().ToString(),
                Name = projectName,
                EventDate = projectDate
            };
            proj.LocalPathRoot = Path.Combine(LocalSettings.SelectedProjectsRootPath, proj.UniqueName);
            projects.Insert(proj);
            Directory.CreateDirectory(proj.LocalPathRoot);
            SaveProject(proj);
            return proj;
        }

        public void ChangeRootDirectory()
        {
            var currentPath = LocalSettings.SelectedProjectsRootPath;
            var allPaths = DriveInfo
                .GetDrives()
                .Select(d => new DirectoryInfo(Path.Combine(d.Name, "projekty")))
                .Where(d => d.Exists)
                .Select(d => d.FullName.TrimEnd('\\').ToLower())
                .ToList();

            var currentPos = allPaths.IndexOf(currentPath);
            var nextPos = currentPos + 1;
            if (nextPos >= allPaths.Count)
                nextPos = 0;
            LocalSettings.SelectedProjectsRootPath = allPaths[nextPos];
            localSettings.Update(LocalSettings);
        }

        public List<AmeFotoVideoProject> AllProjects()
        {
            var rootDir = new DirectoryInfo(LocalSettings.SelectedProjectsRootPath);
            var projesct = rootDir.GetDirectories()
                .Where(d => d.GetFiles("ame-project.json").Any())
                .Select(f => GetProject(f.FullName))
                .ToList();
            return projesct;
        }

        public AmeFotoVideoProject GetProject(string projectPath)
        {
            var projectFilePath = Path.Combine(projectPath, "ame-project.json");
            var json = File.ReadAllText(projectFilePath);
            var project = JsonConvert.DeserializeObject<AmeFotoVideoProject>(json);
            project.LocalPathRoot = projectPath;
            return project;
        }

        public void SaveProject(AmeFotoVideoProject project)
        {
            projects.Update(project);
            var projectFilePath = Path.Combine(project.LocalPathRoot, "ame-project.json");
            File.WriteAllText(projectFilePath, JsonConvert.SerializeObject(project, Formatting.Indented));
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}