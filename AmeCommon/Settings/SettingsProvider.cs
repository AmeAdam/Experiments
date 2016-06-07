using AmeCommon.MediaTasks.Settings;
using System.Collections.Generic;

namespace AmeCommon.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly ICommonSettingsProvider settingsReader;
        readonly CardGrabberSettings settings;

        public SettingsProvider(ICommonSettingsProvider settingsReader)
        {
            this.settingsReader = settingsReader;
            settings = settingsReader.GetSettings<CardGrabberSettings>();
            UserSettings = settingsReader.GetUserSettings<UserSettings>();
        }

        public void SaveUserSettings()
        {
            settingsReader.SaveUserSettings(UserSettings);
        }

        public SvnSettings SvnSettings => settings.SvnSettings;
        public List<CardSettings> CardSettings => settings.Medias;
        public UserSettings UserSettings { get; }
    }
}
