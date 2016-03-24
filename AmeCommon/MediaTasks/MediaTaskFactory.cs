using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AmeCommon.MediaTasks.Settings;
using AmeCommon.MediaTasks.TaskHandlers;
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
                XmlSerializer xs = new XmlSerializer(typeof (AmeCardSettings));
                using (var xr = doc.CreateReader())
                {
                    var acs = (AmeCardSettings) xs.Deserialize(xr);
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

            var profile = settings.Medias.FirstOrDefault(m => m.Id == cardSettings.Id);
            if (profile == null)
                throw new ApplicationException("Could not found profile settings for: " + cardSettings.Id);

            var media = new Media
            {
                Id = cardSettings.Id,
                Name = cardSettings.Name,
                Index = cardSettings.Index,
                SourceDisk = drive,
                Status = EnumMediaStatus.None,
                //DestinationFolder = "?"
            };

            media.TaskHandlers = profile.Tasks.Select(t => CreateTaskHandler(t, media)).ToList();
            return media;
        }

        public IMediaTask CreateTaskHandler(TaskSettings taskSettings, Media parent)
        {
            switch (taskSettings.Name)
            {
                case "move-avchd":
                    return new MoveAvchd(parent, taskSettings);
                case "move-dcim":
                    return new MoveDcim(parent, taskSettings);
                case "move-dcim-canon":
                    return new MoveDcimCanon(parent, taskSettings);
                case "move-zoom":
                    return new MoveZoom(parent, taskSettings);
                default:
                    throw new ApplicationException("Not supported task: " + taskSettings.Name);
            }
        }
    }
}