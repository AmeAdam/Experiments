using System;
using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeCommon.CardsCapture
{
    public interface ICaptureProjectCooridinator
    {
        List<BackgroundTask> GetAavaliableCommands(AmeFotoVideoProject project);
     //   void Execute(CaptureProjectCommand command);
    //    void AppendCommand(AmeFotoVideoProject project, DeviceMoveFileCommands cmd);
   //     void AbortCapture(string uniqueName);
//        DeviceMoveFileCommands GetDevicesCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory);
        Device GetDevice(DriveInfo sourceDrive);
   //     CaptureProjectCommand GetPendingCaptureProjectCommand();
//        CaptureProjectCommand CreateCaptureProjectCommand(AmeFotoVideoProject project, List<DeviceMoveFileCommands> commands);
        Guid StartCapture(AmeFotoVideoProject project, IEnumerable<DriveInfo> drives);
        DeviceMoveFileCommands CreateCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory);
    }
}