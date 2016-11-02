using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeCommon.CardsCapture
{
    public class DeviceMoveFileCommands : BackgroundTask
    {
        public Device Device { get; set; }
        public DriveInfo SourceDrive { get; set; }
        public List<MoveFileCommand> Commands { get; set; }
        public int FilesCount => Commands.Count(cmd => !cmd.Completed);
        public long FilesSize => Commands.Where(cmd => !cmd.Completed).Sum(cmd => cmd.SourceFile.Length);
        public long FilesSizeGb => FilesSize/1024/1024/1024;
        public override string Name => "Kopiowanie plików z " + Device;
        private readonly IDriveManager driveManager;
        public override string Label => Device?.UniqueName;
        public DirectoryInfo DestinationRoot { get; set; }


        public DeviceMoveFileCommands(IDriveManager driveManager)
        {
            this.driveManager = driveManager;
        }

        protected bool Equals(DeviceMoveFileCommands other)
        {
            return Equals(SourceDrive.Name.ToLower(), other.SourceDrive.Name.ToLower());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DeviceMoveFileCommands) obj);
        }

        public override int GetHashCode()
        {
            return SourceDrive?.Name.GetHashCode() ?? 0;
        }

        public int PercentCompleted
        {
            get
            {
                if (Commands == null || Commands.Count == 0)
                    return 100;
                var sumAll = Commands.Sum(c => c.File.Size);
                if (sumAll == 0)
                    return 100;
                var sumCompleted = Commands.Where(c => c.Completed).Sum(c => c.File.Size);
                var progress = (int) (sumCompleted*100/sumAll);
                return progress;
            }
        }


        public IEnumerable<MoveFileCommand> GetAllConflictWithStoredFiles()
        {
            return Commands.Where(c => c.State == TaskState.Waiting)
                .Where(c =>  c.DestinationFile.Exists);
        }

        protected override void Execute()
        {
            if (!driveManager.TryLockDrive(SourceDrive))
                throw new ApplicationException($"Drive {SourceDrive.Name} is locked by other command!");

Thread.Sleep(60000);

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
                driveManager.ReleaseLock(SourceDrive);
            }
        }

        public virtual void DeleteCopiedFiles()
        {
            foreach (var cmd in Commands.Where(cmd => cmd.Completed))
            {
                cmd.DeleteSourceFile();
            }
        }
    }
}