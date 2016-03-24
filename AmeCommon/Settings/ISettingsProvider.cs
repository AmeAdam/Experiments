using System.Collections.Generic;
using AmeCommon.MediaTasks.Settings;

namespace AmeCommon.Settings
{
    public interface ISettingsProvider
    {
        List<MediaSettings> Medias { get; }
        SvnSettings SvnSettings { get; }
        List<TargetFolder> TargetFolders { get; }
    }
}