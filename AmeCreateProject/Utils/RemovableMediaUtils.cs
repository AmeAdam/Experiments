using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmeCreateProject.Utils
{
    public interface IRemovableMediaUtils
    {
        IEnumerable<AmeDriveInfo> GetAllDrives();
    }

    public class RemovableMediaUtils : IRemovableMediaUtils
    {
        public IEnumerable<AmeDriveInfo> GetAllDrives()
        {
            return DriveInfo.GetDrives()
                .Select(d => new AmeDriveInfo
                {
                    Name = d.VolumeLabel,
                    RootPath = d.Name
                });
        }
    }

    public class AmeDriveInfo
    {
        public string RootPath { get; set; }
        public string Name { get; set; }
    }
}
