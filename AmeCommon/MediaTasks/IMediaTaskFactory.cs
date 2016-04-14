using System.IO;

namespace AmeCommon.MediaTasks
{
    public interface IMediaTaskFactory
    {
        Media CreateMedia(DriveInfo drive);
    }
}