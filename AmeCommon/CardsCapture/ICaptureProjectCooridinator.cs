using System;
using System.Collections.Generic;
using System.IO;
using AmeCommon.Model;
using AmeCommon.Tasks;

namespace AmeCommon.CardsCapture
{
    public interface ICaptureProjectCooridinator
    {
        List<DeviceMoveFileCommands> GetAavaliableDeviceCommands(AmeFotoVideoProject project);
     //   void Execute(CaptureProjectCommand command);
    //    void AppendCommand(AmeFotoVideoProject project, DeviceMoveFileCommands cmd);
   //     void AbortCapture(string uniqueName);
//        DeviceMoveFileCommands GetDevicesCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory);
        Device GetDevice(DriveInfo sourceDrive);
        //     CaptureProjectCommand GetPendingCaptureProjectCommand();
        //        CaptureProjectCommand CreateCaptureProjectCommand(AmeFotoVideoProject project, List<DeviceMoveFileCommands> commands);
        Guid StartCapture(AmeFotoVideoProject project, string template, IEnumerable<DriveInfo> drivesToCopy, bool addToSvn);
        DeviceMoveFileCommands CreateCommand(DriveInfo sourceDrive, DirectoryInfo destinationDirectory);
    }
}