using System;
using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AmeCommon.Database
{
    public interface IDatabase
    {
        LiteCollection<AmeFotoVideoProject> Projects { get; }
        LiteCollection<AmeLocalSettings> LocalSettings { get; }
        LiteCollection<Device> Devices { get; }
        void Dispose();
    }

    public class Database : IDisposable, IDatabase
    {
        private readonly IHostingEnvironment environment;
        private readonly LiteDatabase db;
        public LiteCollection<AmeFotoVideoProject> Projects { get; }
        public LiteCollection<AmeLocalSettings> LocalSettings { get; }
        public LiteCollection<Device> Devices { get; }

        public Database(IOptions<AmeConfig> ameConfig, IHostingEnvironment environment)
        {
            this.environment = environment;
            db = new LiteDatabase(ameConfig.Value.LiteDbDatabaseFilePath);
            Devices = db.GetCollection<Device>("devices");
            Projects = db.GetCollection<AmeFotoVideoProject>("projects");
            LocalSettings = db.GetCollection<AmeLocalSettings>("localSettings");
            if (Devices.Count() == 0)
                InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var devicesFilePath = Path.Combine(environment.WebRootPath, "initial-database", "devices.json");
            Devices.Insert(JsonConvert.DeserializeObject<List<Device>>(File.ReadAllText(devicesFilePath)));
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}