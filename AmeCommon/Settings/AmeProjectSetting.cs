using System.Collections.Generic;

namespace AmeCommon.Settings
{
    public class AmeProjectSetting
    {
        public List<AmeProjectFile> Files { get; set; } = new List<AmeProjectFile>();
        public ProjectStateEnum ProjectState { get; set; } = ProjectStateEnum.Unknown;
        public string SvnRootPath { get; set; }
        public string Editor { get; set; }
        public string FtpRootPath { get; set; }

        public enum ProjectStateEnum
        {
            Unknown,
            TransferingInProgress,
            TransferingComplete,
            Editing,
            Completed,
            Archived,
        }
    }

    public class AmeProjectFile
    {
        public string RelativePath { get; set; }
        public string OriginalName { get; set; }
        public long Size { get; set; }
        public string Md5 { get; set; }
    }
}
