using System.IO;

namespace AmeCommon.Model
{
    public class MediaFile
    {
        public string RelativePath { get; set; }
        public string CheckSum { get; set; }
        public string Source { get; set; }
        public long Size { get; set; }

        public FileInfo GetDestinationFile(DirectoryInfo destinationRoot)
        {
            var absoluteFilePath = Path.Combine(destinationRoot.FullName, RelativePath);
            return new FileInfo(absoluteFilePath);
        }
    }
}