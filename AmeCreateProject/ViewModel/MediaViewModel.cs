using AmeCommon.MediaTasks;
using Microsoft.Practices.Prism.Mvvm;
using System.IO;
using System.Windows;

namespace AmeCreateProject.ViewModel
{
    public class MediaViewModel : BindableBase
    {
        private Media media;
        private string lastMessage = "";

        public MediaViewModel(Media media)
        {
            this.media = media;
            media.StatusChanged += OnStatusChanged;
            IsActive = media.SourceDisk.DriveType == DriveType.Removable;
        }

        private void OnStatusChanged(Media parent, EnumMediaStatus status, string message)
        {
            lastMessage = message ?? "";
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(ImgProgressVisible));
            OnPropertyChanged(nameof(ImgDoneVisible));
            OnPropertyChanged(nameof(ImgErrorVisible));
        }

        public bool IsActive { get; set; }

        public Media Tag {  get { return media; } }

        public string Description { get { return media.Id + " " + media.Name; } }

        public string Message
        {
            get
            {
                return lastMessage;
            }
        }

        public Visibility ImgProgressVisible
        {
            get
            {
                return media.Status == EnumMediaStatus.InProgress ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ImgDoneVisible
        {
            get
            {
                return media.Status == EnumMediaStatus.Completed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ImgErrorVisible
        {
            get
            {
                return media.Status == EnumMediaStatus.Failed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

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
}
