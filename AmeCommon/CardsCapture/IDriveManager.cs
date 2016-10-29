using System.Collections.Generic;
using System.IO;

namespace AmeCommon.CardsCapture
{
    public interface IDriveManager
    {
        bool TryLockDrive(DriveInfo drive);
        void ReleaseLock(DriveInfo drive);
        List<DriveInfo> GetAvaliableRemovableDrives();
        List<DriveInfo> GetAllRemovableDrives();
    }
}