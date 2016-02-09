using System.Collections.Generic;
using System.IO;
using CardGrabberCmd.MediaTasks;

namespace CardGrabberCmd
{
    public class Media
    {
        public const string AmeNamespace = "http://kamerzysta.bydgoszcz.pl";
        public string Id { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public DriveInfo SourceDisk { get; set; }
        public DirectoryInfo DestinationFolder { get; set; }

        public List<IMediaTask> Tasks  { get; set; }
    }

    public class MediaFactory
    {
        xxx
    }
}
