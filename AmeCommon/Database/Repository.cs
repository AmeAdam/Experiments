using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;
using LiteDB;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AmeCommon.Database
{
    public class Repository : IDeviceRepository, IAmeProjectRepository, IDisposable
    {
        private object sync = new object();
        public const string AmeProjectFileName = "ame-project.json";
        private LiteDatabase db;
        private readonly LiteCollection<AmeFotoVideoProject> projects;
        private readonly LiteCollection<Device> devices;
        private readonly LiteCollection<AmeLocalSettings> localSettings;
        public AmeLocalSettings LocalSettings { get; set; }

        public Repository(IOptions<AmeConfig> ameConfig)
        {
            db = new LiteDatabase(ameConfig.Value.LiteDbDatabaseFilePath);
            devices = db.GetCollection<Device>("devices");
            projects = db.GetCollection<AmeFotoVideoProject>("projects");
            localSettings = db.GetCollection<AmeLocalSettings>("localSettings");
            LocalSettings = localSettings.FindById(1);
            if (LocalSettings == null)
            {
                LocalSettings = new AmeLocalSettings {SelectedProjectsRootPath = ameConfig.Value.DefaultProjectPath, Id = 1};
                localSettings.Insert(LocalSettings);
            }
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

        public int CreateProject(DateTime projectDate, string projectName)
        {
            var proj = new AmeFotoVideoProject
            {
                Name = projectName,
                EventDate = projectDate,
            };
            proj.LocalPathRoot = Path.Combine(LocalSettings.SelectedProjectsRootPath, proj.UniqueName);
            return projects.Insert(proj);
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
            return projects.FindAll().ToList();
        }

        public AmeFotoVideoProject GetProject(int id)
        {
            return projects.FindById(id);
        }

        public void RemoveProject(int projectId)
        {
            projects.Delete(projectId);
        }

        public void SaveProject(AmeFotoVideoProject project)
        {
            projects.Update(project);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}