using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmeCommon.MediaTasks
{
    public class MediaService
    {
        private object sync = new object();
        private List<Media> allMedias;
        private MediaTaskFactory factory;

        public MediaService()
        {
            factory = new MediaTaskFactory();
        }

        public List<Media> GetAllMedias()
        {
            return DriveInfo.GetDrives()
                .Select(factory.CreateMedia)
                .Where(m => m != null)
                .ToList();
        }
    }
}
