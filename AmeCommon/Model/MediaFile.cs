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

        protected bool Equals(MediaFile other)
        {
            return string.Equals(RelativePath?.ToLower(), other.RelativePath?.ToLower());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MediaFile) obj);
        }

        public override int GetHashCode()
        {
            return (RelativePath != null ? RelativePath.GetHashCode() : 0);
        }
    }
}