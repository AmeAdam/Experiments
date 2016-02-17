using System.Collections.Generic;
using System.IO;
using CardGrabberCmd.MediaTasks.TaskHandlers;
using System;
using System.Threading.Tasks;

namespace CardGrabberCmd.MediaTasks
{
    public class Media
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public DriveInfo SourceDisk { get; set; }
        public DirectoryInfo DestinationFolder { get; set; }
        public List<IMediaTask> TaskHandlers { get; set; }
        public EnumMediaStatus Status { get; set; } = EnumMediaStatus.None;
        public event Action<Media, EnumMediaStatus, string> StatusChanged;

        public Task ExecuteAllTasks()
        {
            return Task.Factory.StartNew(ExecuteAllTasksInternal);
        }

        private void SetStatus(EnumMediaStatus newStatus, string message, Action<Media, EnumMediaStatus, string> handler)
        {
            Status = newStatus;
            try
            {
                if (handler != null)
                    handler(this, newStatus, message);
            }
            catch (ApplicationException)
            {
                return;
            }
        }

        private void ExecuteAllTasksInternal()
        {
            SetStatus(EnumMediaStatus.InProgress, "przetwarzanie", StatusChanged);
            try
            {
                foreach (var handler in TaskHandlers)
                    handler.Execute();
                SetStatus(EnumMediaStatus.Completed, "zakończono", StatusChanged);
            }
            catch(ApplicationException ex)
            {
                SetStatus(EnumMediaStatus.Failed, ex.Message, StatusChanged);
            }
        }        
    }

    public enum EnumMediaStatus
    {
        None,
        InProgress,
        Completed,
        Failed
    }

}
