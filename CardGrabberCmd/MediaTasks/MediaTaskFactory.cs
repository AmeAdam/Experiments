using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CardGrabberCmd.MediaTasks.Settings;
using CardGrabberCmd.MediaTasks.TaskHandlers;

namespace CardGrabberCmd.MediaTasks
{
    public class MediaTaskFactory
    {
        private readonly CardProfilesSettings profiles;

        public MediaTaskFactory()
        {
            var doc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cards-profiles.xml"));
            XmlSerializer xs = new XmlSerializer(typeof(CardProfilesSettings));
            using (var xr = doc.CreateReader())
            {
                profiles = (CardProfilesSettings)xs.Deserialize(xr);
            }
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
        
        public Media CreateMedia(DriveInfo drive, AmeCardSettings cardSettings)
        {
            var profile = profiles.Medias.FirstOrDefault(m => m.Id == cardSettings.Id);
            if (profile == null)
                throw new ApplicationException("Could not found profile settings for: " + cardSettings.Id);

            var media = new Media
            {
                Id = cardSettings.Id,
                Name = cardSettings.Name,
                Index = cardSettings.Index,
                SourceDisk = drive,
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
                default:
                    throw new ApplicationException("Not supported task: " + taskSettings.Name);
            }
        }
    }
}