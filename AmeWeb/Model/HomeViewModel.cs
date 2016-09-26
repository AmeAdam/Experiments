using System.Collections.Generic;
using System.IO;
using AmeCommon.CardsCapture;
using AmeCommon.Model;

namespace AmeWeb.Model
{
    public class HomeViewModel
    {
        //public AmeFotoVideoProject CurrentProject { get; set; }
        public List<AmeFotoVideoProject> KnownProjects { get; set; }
        //public List<DeviceMoveFileCommands> Devices { get; set; }
        public string DestiantionRoot { get; set; }
        //public long AvailableFreeSpace => new DriveInfo(Path.GetPathRoot(DestiantionRoot)??"c:").AvailableFreeSpace/1024/1024/1024;
    }
}
