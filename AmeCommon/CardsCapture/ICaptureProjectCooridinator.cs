using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public interface ICaptureProjectCooridinator
    {
        List<DeviceMoveFileCommands> GetAllDevicesCommand(IEnumerable<DriveInfo> drives, DirectoryInfo destinationDirectory);
        void Execute(CaptureProjectCommand command);
        void AppendCommand(AmeFotoVideoProject project, DeviceMoveFileCommands cmd);
        void AbortCapture(string uniqueName);
        DeviceMoveFileCommands GetDevicesCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory);
        Device GetDevice(DriveInfo sourceDrive);
        CaptureProjectCommand GetPendingCaptureProjectCommand();
    }
}