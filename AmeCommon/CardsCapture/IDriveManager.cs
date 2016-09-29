using System.Collections.Generic;
using System.IO;

namespace AmeCommon.CardsCapture
{
    public interface IDriveManager
    {
        void LockDrive(DriveInfo drive);
        void UnlockDrive(DriveInfo drive);
        List<DriveInfo> GetAvaliableRemovableDrives();
        List<DriveInfo> GetAllRemovableDrives();
    }
}