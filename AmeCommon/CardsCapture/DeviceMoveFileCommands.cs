using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public class DeviceMoveFileCommands
    {
        public Device Device { get; set; }
        public DriveInfo SourceDrive { get; set; }
        public List<MoveFileCommand> Commands { get; set; }
        public bool IsCompleted { get; set; }
        public int FilesCount => Commands.Count(cmd => !cmd.Completed);
        public long FilesSize => Commands.Where(cmd => !cmd.Completed).Sum(cmd => cmd.SourceFile.Length);
        public long FilesSizeGb => FilesSize/1024/1024/1024;
        public DeviceCommandState State { get; set; } = DeviceCommandState.Waiting;
        private readonly Task worker;
        public string Message { get; set; }
        private volatile bool abort;
        private Action<DeviceMoveFileCommands> onCompleteAction;
        private volatile MoveFileCommand pendingCommand;

        public DeviceMoveFileCommands()
        {
            worker = new Task(ExecuteInternal);
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

        public enum DeviceCommandState
        {
            Waiting,
            InProgress,
            Completed,
            Error
        }

        public void SetDestinationRootPath(DirectoryInfo projectLocalRoot)
        {
            Commands.ForEach(cmd => cmd.DestinationRoot = projectLocalRoot);
        }

        public void ExecuteAsync(Action<DeviceMoveFileCommands> onComplete)
        {
            onCompleteAction = onComplete;
            worker.Start();
        }

        private void ExecuteInternal()
        {
            State = DeviceCommandState.InProgress;
            foreach (var cmd in Commands)
            {
                if (abort)
                    return;
                try
                {
                    pendingCommand = cmd;
                    cmd.Execute();
                    pendingCommand = null;
                }
                catch (Exception ex)
                {
                    Message = ex.ToString();
                    State = DeviceCommandState.Error;
                }
            }
            if (State != DeviceCommandState.Error)
                State = DeviceCommandState.Completed;
            IsCompleted = true;
            onCompleteAction?.Invoke(this);
        }

        public void Abort()
        {
            if (worker == null)
                return;
            abort = true;
            var pending = pendingCommand;
            if (pending != null)
                pending.Abort();
            worker.Wait();
            abort = false;
            State = DeviceCommandState.Error;
            Message = "Operacja przerwana przez użytkownika";
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