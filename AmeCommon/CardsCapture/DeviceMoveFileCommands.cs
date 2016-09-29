using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeCommon.CardsCapture
{
    public class DeviceMoveFileCommands : BackgroundTask
    {
        private readonly IDriveManager driveManager;
        public Device Device { get; set; }
        public DriveInfo SourceDrive { get; set; }
        public List<MoveFileCommand> Commands { get; set; }
        public bool IsCompleted { get; set; }
        public int FilesCount => Commands.Count(cmd => !cmd.Completed);
        public long FilesSize => Commands.Where(cmd => !cmd.Completed).Sum(cmd => cmd.SourceFile.Length);
        public long FilesSizeGb => FilesSize/1024/1024/1024;

        public DeviceMoveFileCommands(IDriveManager driveManager)
        {
            this.driveManager = driveManager;
            Name = "Kopiowanie plików z " + SourceDrive;
        }

        public int PercentCompleted
        {
            get
            {
                if (Commands == null || Commands.Count == 0)
                    return 100;
                var sumAll = Commands.Sum(c => c.File.Size);
                var sumCompleted = Commands.Where(c => c.Completed).Sum(c => c.File.Size);
                var progress = (int) (sumCompleted*100/sumAll);
                return progress;
            }
        }


        public IEnumerable<MoveFileCommand> GetAllConflictWithStoredFiles()
        {
            return Commands.Where(c => !c.Completed)
                .Where(c => c.DestinationFile.Exists);
        }

        public void SetDestinationRootPath(DirectoryInfo projectLocalRoot)
        {
            Commands.ForEach(cmd => cmd.DestinationRoot = projectLocalRoot);
        }

        protected override void Execute()
        {
            driveManager.LockDrive(SourceDrive);
            try
            {
                foreach (var cmd in Commands)
                {
                    if (CancellationToken.IsCancellationRequested)
                        return;
                    cmd.Execute(CancellationToken);
                }
            }
            finally
            {
                driveManager.UnlockDrive(SourceDrive);
            }
        }

        public void DeleteCopiedFiles()
        {
            foreach (var cmd in Commands.Where(cmd => cmd.Completed))
            {
                cmd.DeleteSourceFile();
            }
        }
    }
}