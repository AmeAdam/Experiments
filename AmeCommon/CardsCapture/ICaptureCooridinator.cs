using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;

namespace AmeCommon.CardsCapture
{
    public interface ICaptureCooridinator
    {
        IEnumerable<DeviceMoveFileCommands> GetAllDevicesCommand(IEnumerable<DriveInfo> drives);
        void Execute(CaptureProjectCommand command);
        void AppendCommand(AmeFotoVideoProject project, DeviceMoveFileCommands cmd);
        void AbortCapture(string uniqueName);
        DeviceMoveFileCommands GetDevicesCommand(DriveInfo sourceDrive);
        Device GetDevice(DriveInfo sourceDrive);
    }
}