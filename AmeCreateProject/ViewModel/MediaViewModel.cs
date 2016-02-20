using AmeCommon.MediaTasks;
using AmeCreateProject.Resources;
using Microsoft.Practices.Prism.Mvvm;
using System.IO;
using System.Reflection;

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
        }

        private void OnStatusChanged(Media parent, EnumMediaStatus status, string message)
        {
            lastMessage = message ?? "";
            OnPropertyChanged(nameof(Message));
        }

        public string Description { get { return media.Id + " " + media.Name; } }
        public string Message { get { return media.Status.ToString() + " " + lastMessage; } }
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
