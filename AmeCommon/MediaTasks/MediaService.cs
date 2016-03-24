using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmeCommon.MediaTasks
{
    public class MediaService : IMediaService
    {
        private object sync = new object();
        private List<Media> allMedias;
        private IMediaTaskFactory factory;

        public MediaService(IMediaTaskFactory factory)
        {
            this.factory = factory;
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
