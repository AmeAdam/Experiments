using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeWeb.Model
{
    public class CaptureCardsViewModel
    {
        public AmeFotoVideoProject Project { get; set; }

        public long AvailableFreeDiskSpace
        {
            get
            {
                var root = new DriveInfo(Path.GetPathRoot(Project.LocalPathRoot) ?? "c:");
                return root.AvailableFreeSpace;
            }
        }

        public long RequiredDiskSpace
        {
            get { return AvaliableCommands.OfType<DeviceMoveFileCommands>().Sum(cmd => cmd.FilesSize); }
        }

        public List<TaskViewModel> AvaliableCommands { get; set; }
    }

    public class TaskViewModel
    {
        public bool Selected { get; set; }
        public string CommandImagePath { get; set; }
        public bool HasWarning { get; set; }
        public string Label { get; set; }
        public string SourceDrive { get; set; }
        public string Description { get; set; }
        public TaskState State { get; set; }
        public string StateImagePath { get; set; }
        public string ExecuteActionLink { get; set; }
    }
}
