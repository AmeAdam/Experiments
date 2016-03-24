using System.IO;
using AmeCommon.MediaTasks.Settings;
using AmeCommon.MediaTasks.TaskHandlers;

namespace AmeCommon.MediaTasks
{
    public interface IMediaTaskFactory
    {
        Media CreateMedia(DriveInfo drive);
        IMediaTask CreateTaskHandler(TaskSettings taskSettings, Media parent);
    }
}