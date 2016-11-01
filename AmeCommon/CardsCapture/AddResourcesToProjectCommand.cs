using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;
using Microsoft.AspNetCore.Hosting;

namespace AmeCommon.CardsCapture
{
    public class AddResourcesToProjectCommand : DeviceMoveFileCommands
    {
        private readonly IHostingEnvironment environment;
        private readonly DirectoryInfo destinationDirectory;
        public override string Name => "Tworzenie plików dodatkowych projektu";
        public override string Label => "resources";


        public AddResourcesToProjectCommand(IHostingEnvironment environment, DirectoryInfo destinationDirectory, IDriveManager driveManager) : base(driveManager)
        {
            this.environment = environment;
            this.destinationDirectory = destinationDirectory;
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
            var podkladyDestPath = Path.Combine(destinationDirectory.FullName, "Podkłady");
            Directory.CreateDirectory(podkladyDestPath);
            foreach(var podklad in Directory.GetFiles(sourceFile))
                File.Copy(podklad, Path.Combine(podkladyDestPath, Path.GetFileName(podklad) ?? ""));
        }

        private void CopyFile(string fileName)
        {
            var sourceFile = Path.Combine(environment.WebRootPath, "Resources", fileName);
            var destFile = Path.Combine(destinationDirectory.FullName, fileName);
            File.Copy(sourceFile, destFile);
        }

        public override void DeleteCopiedFiles()
        {
        }
    }
}