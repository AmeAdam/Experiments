using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmeCommon.CardsCapture;
using AmeCommon.Model;

namespace AmeWeb.Model
{
    public class CaptureCardsViewModel
    {
        public AmeFotoVideoProject Project { get; set; }
        public CaptureProjectCommand PendingTask { get; set; }
        public List<DeviceMoveFileCommands> AvaliableCommands { get; set; }

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
            get { return AvaliableCommands.Sum(cmd => cmd.FilesSize); }
        }
    }
}
