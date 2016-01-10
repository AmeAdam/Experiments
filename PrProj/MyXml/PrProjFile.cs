using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyXml.MyXml;
using MyXml.PrProjXmlCommands;
using MyXml.PrProj;
using System.Xml;
using System.Xml.Linq;
using System;

namespace MyXml
{
    public class PrProjFile
    {
        private readonly string filePath;
        public const long Multiply = 10160640000L;// / 2;
        private readonly XDocument doc;
        public Dictionary<string, ClipProj> Clips = new Dictionary<string, ClipProj>();
        //public EffectRequest EffectRequest;
        public List<Sequence> Sequences;

        public PrProjFile(string filePath)
        {
            this.filePath = filePath;
            doc = XDocument.Load(filePath);
           // EffectRequest = new EffectRequest(doc);
           // Sequences = doc.Root.Elements("Sequence").Select(s => new Sequence(s)).ToList();
           // Sequences.Add(new Sequence { Id = "", Name = "All" });
        }

        public void Save()
        {
            var bakPath = GetFreeBackPath(filePath + ".bak");

            File.Move(filePath, bakPath);
            using (var file = File.OpenWrite(filePath))
            {
                doc.Save(file);
            }
        }

        private string GetFreeBackPath(string path)
        {
            if (!File.Exists(path))
                return path;
            var ext = Path.GetExtension(path).Split('-');
            var index = 0;
            if (ext.Length > 1)
                int.TryParse(ext[1], out index);
            
            for (int i = index; ; i++)
            {
                var newPath = path + "-" + i;
                if (!File.Exists(newPath))
                    return newPath;
            }
        }

        public void Analize()
        {
            foreach (var s in doc.Root.Elements("VideoClipTrackItem"))
            {
                var name = GetClipPath(s);
                ClipProj clip;
                if (!Clips.TryGetValue(name, out clip))
                {
                    clip = new ClipProj(name);
                    Clips[name] = clip;
                }
                clip.AddTrack(s);
            }

            foreach (var s in doc.Root.Elements("AudioClipTrackItem"))
            {
                var name = GetClipPath(s); // +"_" + GetClipTrackIndex(s);
                ClipProj clip;
                if (!Clips.TryGetValue(name, out clip))
                {
                    clip = new ClipProj(name);
                    Clips[name] = clip;
                }
                clip.AddTrack(s);
            }
            
            /*
            var seq = doc.Root.Find("PremiereData", "VideoClipTrackItem");
            foreach (Element s in seq)
            {
                var name = GetClipPath(s);
                ClipProj clip;
                if (!Clips.TryGetValue(name, out clip))
                {
                    clip = new ClipProj(name);
                    Clips[name] = clip;
                }
                clip.AddTrack(s);
            }

            seq = doc.Root.Find("PremiereData", "AudioClipTrackItem");
            foreach (Element s in seq)
            {
                var name = GetClipPath(s); // +"_" + GetClipTrackIndex(s);
                ClipProj clip;
                if (!Clips.TryGetValue(name, out clip))
                {
                    clip = new ClipProj(name);
                    Clips[name] = clip;
                }
                clip.AddTrack(s);
            }*/
        }

        private string GetClipPath(XElement elem)
        {
            var refElement = elem.Elements("ClipTrackItem").Elements("SubClip").FirstOrDefault();
            if (refElement == null)
                return null;
            var refAttr = (string)refElement.Attribute("ObjectRef");
            if (refAttr == null)
                return null;
            var subClipElem = FindElementById("SubClip", refAttr);
            if (subClipElem == null)
                return null;

            var clipId = (string)subClipElem.Element("Clip").Attribute("ObjectRef");
            var clip = FindElementById("VideoClip", clipId);
            if (clip == null)
                clip = FindElementById("AudioClip", clipId);
            if (clip == null)
                return null;

            var source = clip.Elements("Clip").Elements("Source").FirstOrDefault();
            var mediaRef = (string)source.Attribute("ObjectRef");

            var avMediaSource = FindElementById("VideoMediaSource", mediaRef);
            if (avMediaSource == null)
                avMediaSource = FindElementById("AudioMediaSource", mediaRef);
             if (avMediaSource == null)
                 return null;
             var mediaId = (string)avMediaSource.Elements("MediaSource").Elements("Media").FirstOrDefault().Attribute("ObjectURef");
             var media = FindElementById("Media", mediaId);
             var rawPath =  ((string)media.Element("ActualMediaFilePath")).ToLower();
             rawPath = rawPath.Replace(@"\\?\", "");
             return rawPath.ToLower();
        }

        private XElement FindElementById(string elementName, string elementId)
        {
            return doc.Root.Elements(elementName).FirstOrDefault(e => (string)e.Attribute("ObjectID") == elementId || (string)e.Attribute("ObjectUID") == elementId);
        }

        private string GetClipTrackNameFrmoSubclip(Element elem)
        {
            return null;

            //var refElement = elem.Find("ClipTrackItem","SubClip").FirstOrDefault();
            //if (refElement == null)
            //    return null;
            //var refAttr = refElement.Attributes.Find(a => a.Name == "ObjectRef");
            //if (refAttr == null)
            //    return null;
            //var subClipElem = doc.Root.Find("PremiereData", "SubClip").FirstOrDefault(e => e.Attributes.Exists(a => a.Name == "ObjectID" && a.Value == refAttr.Value));
            //if (subClipElem == null)
            //    return null;
            //var nameElem = subClipElem.Find("Name").FirstOrDefault();
            //if (nameElem == null)
            //    return null;
            //return nameElem.InnerText;
        }

        public void Update(string name, long startFrame, long endFrame)
        {
            ClipProj clip;
            if (!Clips.TryGetValue(name, out clip))
                return;
            clip.UpdateTimes(startFrame, endFrame);
        }


    }
}
