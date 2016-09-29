using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmeCommon.CardsCapture
{
    public class DriveManager : IDriveManager
    {
        private readonly object sync = new object();
        private readonly HashSet<string> lockedDrives = new HashSet<string>();

        public void LockDrive(DriveInfo drive)
        {
            lock (sync)
            {
                lockedDrives.Add(drive.Name.ToLower());
            }
        }

        public void UnlockDrive(DriveInfo drive)
        {
            lock (sync)
            {
                lockedDrives.Remove(drive.Name.ToLower());
            }
        }

        public List<DriveInfo> GetAvaliableRemovableDrives()
        {
            lock (sync)
            {
                return DriveInfo.GetDrives()
                    .Where(d => d.DriveType == DriveType.Removable)
                    .Select(d => d.Name.ToLower())
                    .Except(lockedDrives)
                    .Select(name => new DriveInfo(name))
                    .ToList();
            }
        }

        public List<DriveInfo> GetAllRemovableDrives()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Removable)
                .ToList();
        }
    }
}
