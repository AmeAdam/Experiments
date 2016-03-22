using System.Collections.Generic;
using System.IO;
using AmeCommon.MediaTasks.TaskHandlers;
using System;
using System.Threading.Tasks;

namespace AmeCommon.MediaTasks
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

        public Task ExecuteAllTasksAsync()
        {
            return Task.Factory.StartNew(ExecuteAllTasksInternal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;
            return Equals(obj as Media);
        }

        public bool Equals(Media other)
        {
            if (ReferenceEquals(other, this))
                return true;
            if (ReferenceEquals(null, other))
                return false;
            return Id == other.Id && Name == other.Name && Index == other.Index;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Id != null ? Id.GetHashCode() : 0;
                result = (result * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result * 397) ^ (Index != null ? Index.GetHashCode() : 0);
                return result;
            }
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
        None = 0,
        Empty = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4
    }

}
