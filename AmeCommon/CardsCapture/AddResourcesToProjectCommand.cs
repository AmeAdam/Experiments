﻿using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;
using Microsoft.AspNetCore.Hosting;

namespace AmeCommon.CardsCapture
{
    public class AddResourcesToProjectCommand : DeviceMoveFileCommands
    {
        private readonly IHostingEnvironment environment;
        private readonly AmeFotoVideoProject project;

        public AddResourcesToProjectCommand(IHostingEnvironment environment, AmeFotoVideoProject project)
        {
            this.environment = environment;
            this.project = project;
            Device = new Device {UniqueName = "Startup"};
            SourceDrive = new DriveInfo(Path.GetPathRoot(environment.WebRootPath) ?? "c:");
            Commands = new List<MoveFileCommand>();
        }

        protected override void Execute()
        {
            CopyFile("nadruk_plyta.psd");
            CopyFile("nadruk_pudelko.psd");
            CopyFile("temp.prproj");
            var sourceFile = Path.Combine(environment.WebRootPath, "Resources", "Podkłady");
            var podkladyDestPath = Path.Combine(project.LocalPathRoot, "Podkłady");
            Directory.CreateDirectory(podkladyDestPath);
            foreach(var podklad in Directory.GetFiles(sourceFile))
                File.Copy(podklad, Path.Combine(podkladyDestPath, Path.GetFileName(podklad) ?? ""));
        }

        private void CopyFile(string fileName)
        {
            var sourceFile = Path.Combine(environment.WebRootPath, "Resources", fileName);
            var destFile = Path.Combine(project.LocalPathRoot, fileName);
            File.Copy(sourceFile, destFile);
        }

        public override void DeleteCopiedFiles()
        {
        }
    }
}