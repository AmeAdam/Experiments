using AmeCommon.MediaTasks;
using Microsoft.Practices.Prism.Mvvm;

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
            OnPropertyChanged(nameof(Status));
        }

        public string Name { get { return media.Id; } }
        public string DestinationFolderName { get { return media.Name; } }
        public string Status { get { return media.Status.ToString() + " " + lastMessage; } }

    }
}
