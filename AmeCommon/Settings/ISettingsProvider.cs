using System.Collections.Generic;
using AmeCommon.MediaTasks.Settings;

namespace AmeCommon.Settings
{
    public interface ISettingsProvider
    {
        List<CardSettings> CardSettings { get; }
        SvnSettings SvnSettings { get; }
        UserSettings UserSettings { get; }
        void SaveUserSettings();
    }
}