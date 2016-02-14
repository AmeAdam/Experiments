using System.Collections.Generic;
using System.IO;
using CardGrabberCmd.MediaTasks.TaskHandlers;

namespace CardGrabberCmd.MediaTasks
{
    public class Media
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public DriveInfo SourceDisk { get; set; }
        public DirectoryInfo DestinationFolder { get; set; }
        public List<IMediaTask> Tasks { get; set; }
    }
}
