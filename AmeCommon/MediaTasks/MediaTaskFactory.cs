using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AmeCommon.MediaTasks.MoveFilesCommands;
using AmeCommon.MediaTasks.Settings;
using AmeCommon.Settings;

namespace AmeCommon.MediaTasks
{
    public class MediaTaskFactory : IMediaTaskFactory
    {
        private readonly ISettingsProvider settings;

        public MediaTaskFactory(ISettingsProvider settings)
        {
            this.settings = settings;
        }

        private static AmeCardSettings ReadCardSettings(DriveInfo drive)
        {
            var ameFilePath = Path.Combine(drive.Name, "ame.xml");
            if (!File.Exists(ameFilePath))
                return null;

            try
            {
                var doc = XDocument.Load(ameFilePath);
                XmlSerializer xs = new XmlSerializer(typeof(AmeCardSettings));
                using (var xr = doc.CreateReader())
                {
                    var acs = (AmeCardSettings)xs.Deserialize(xr);
                    return acs;
                }
            }
            catch (XmlException)
            {
                return null;
            }
        }

        public Media CreateMedia(DriveInfo drive)
        {
            var cardSettings = ReadCardSettings(drive);
            if (cardSettings == null)
                return null;

            var profile = settings.CardSettings.FirstOrDefault(m => m.Id == cardSettings.Id);
            if (profile == null)
                throw new ApplicationException("Could not found profile settings for: " + cardSettings.Id);

            var media = new Media
            {
                Id = cardSettings.Id,
                Name = cardSettings.Name,
                Index = cardSettings.Index,
                SourceDisk = drive,
                Status = EnumMediaStatus.None,
                Operations = profile.Tasks.Select(t => CreateTaskHandler(t, drive)).ToList()
            };
            return media;
        }

        private IIOCommand CreateTaskHandler(CommandSettings taskSettings, DriveInfo sourceDrive)
        {
            var sourcePath = Path.Combine(sourceDrive.Name, taskSettings.Source);

            switch (taskSettings.Name)
            {
                case "move-directory-content":
                    return new MoveDirectoryContent(SourceDirectory.Create(sourcePath), taskSettings.Target);
                case "move-directory-content-flat":
                    return new MoveDirectoryContentFlat(SourceDirectory.Create(sourcePath), taskSettings.Target);
                case "move-files":
                    return new MoveFiles(SourceDirectory.GetFilesList(sourcePath), taskSettings.Target);
                default:
                    throw new ApplicationException("Not supported command: " + taskSettings.Name);
            }
        }
    }
}