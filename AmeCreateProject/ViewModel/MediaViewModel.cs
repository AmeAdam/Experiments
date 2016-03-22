using AmeCommon.MediaTasks;
using Microsoft.Practices.Prism.Mvvm;
using System.IO;

namespace AmeCreateProject.ViewModel
{
    public class MediaViewModel : BindableBase
    {
        private Media media;

        public MediaViewModel(Media media)
        {
            this.media = media;
            media.StatusChanged += OnStatusChanged;
            IsActive = media.SourceDisk.DriveType == DriveType.Removable;
        }

        private void OnStatusChanged(Media parent, EnumMediaStatus status, string message)
        {
            Message = message ?? "";
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(Status));
        }

        public bool IsActive { get; set; }

        public Media Tag { get { return media; } }

        public string Description { get { return media.Id + " " + media.Name; } }

        public string Message { get; private set; }

        public EnumMediaViewStatus Status { get { return (EnumMediaViewStatus)(media.Status); } }

        public string ImagePath
        {
            get
            {
                var path = Path.GetFullPath(@"Images/"+media.Id+".jpg");
                if (File.Exists(path))
                    return path;
                return "/Resources/unknown.png";
            }
        }
    }

    public enum EnumMediaViewStatus
    {
        None = 0,
        Empty = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4
    }
}
