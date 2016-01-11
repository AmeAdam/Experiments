using System.Collections.Generic;
using System.Linq;
using MyXml.MyXml;
using System.Xml.Linq;

namespace MyXml.PrProj
{
    public class ProjReader
    {
        public Dictionary<string, Sequence> Sequences = new Dictionary<string, Sequence>();
        private readonly Dictionary<string, VideoSequenceSource> videoSequenceSources = new Dictionary<string, VideoSequenceSource>(); //ObjectID="323"
        private readonly Dictionary<string, VideoClip> videoClips = new Dictionary<string, VideoClip>();
        private readonly Dictionary<string, SubClip> subClips = new Dictionary<string, SubClip>();
        private readonly Dictionary<string, VideoClipTrackItem> videoClipTrackItems = new Dictionary<string, VideoClipTrackItem>();

        public void Load(Element root)
        {
         //   LoadSequences(root.Find("PremiereData", "Sequence"));
            LoadVideoSequenceSource(root.Find("PremiereData", "VideoSequenceSource"));
            LoadVideoClips(root.Find("PremiereData", "VideoClip"));
            LoadSubClips(root.Find("PremiereData", "SubClip"));
            LoadVideoClipTrackItems(root.Find("PremiereData", "VideoClipTrackItem"));
        }

        private void LoadVideoClipTrackItems(IEnumerable<Element> list)
        {
            foreach (var elem in list)
            {
                var id = elem.GetAttribute("ObjectID");
                if (id == null)
                    continue;
                var clipTrackItem = elem.Find("ClipTrackItem").FirstOrDefault();
                if (clipTrackItem == null)
                    continue;

                var subClipNode = clipTrackItem.Find("SubClip").FirstOrDefault();
                if (subClipNode == null)
                    continue;
                var subClipId = subClipNode.GetAttribute("ObjectRef");
                SubClip subClip;
                if (!subClips.TryGetValue(subClipId, out subClip))
                    continue;


                var track = new VideoClipTrackItem
                {
                    Element = elem,
                    ObjectID = id,
                    SubClip = subClip,
                };

                var trackItem = clipTrackItem.Find("TrackItem").FirstOrDefault();
                if (trackItem == null)
                    continue;

                long.TryParse(trackItem.GetChildNodeValue("End"), out track.End);
                long.TryParse(trackItem.GetChildNodeValue("Start"), out track.Start);

                subClip.VideoClipTrackItem = track;
                videoClipTrackItems[id] = track;
            }
        }

        private void LoadSubClips(IEnumerable<Element> list)
        {
            foreach (var elem in list)
            {
                var id = elem.GetAttribute("ObjectID");
                if (id == null)
                    continue;

                var clip = elem.Find("Clip").FirstOrDefault();
                if (clip == null)
                    continue;
                var clipId = clip.GetAttribute("ObjectRef");

                VideoClip videoClip;
                if (!videoClips.TryGetValue(clipId, out videoClip))
                    continue;

                var subClip = new SubClip
                {
                    Element = elem,
                    ObjectID = id,
                    Clip = videoClip,
                    Name = elem.GetChildNodeValue("Name"),
                };
                videoClip.SubClip = subClip;
                subClips[id] = subClip;
            }
        }

        private void LoadVideoSequenceSource(IEnumerable<Element> list)
        {
            foreach (var elem in list)
            {
                var id = elem.GetAttribute("ObjectID");
                if (id == null)
                    continue;

                var seq = elem.Find("SequenceSource", "Sequence").FirstOrDefault();
                if (seq == null)
                    continue;
                var seqId = seq.GetAttribute("ObjectURef");

                Sequence seqNode;
                if (!Sequences.TryGetValue(seqId, out seqNode))
                    continue;

                var vss = new VideoSequenceSource
                {
                    Element = elem,
                    ObjectId = id,
                    Sequence = seqNode,
                    Category = SequenceSourceCategory.Video,
                };
                long.TryParse(elem.GetChildNodeValue("OriginalDuration"), out vss.OriginalDuration);
                seqNode.Sources.Add(vss);
                videoSequenceSources[id] = vss;
            }
        }

        private void LoadVideoClips(IEnumerable<Element> list)
        {
            foreach (var elem in list)
            {
                var id = elem.GetAttribute("ObjectID");
                if (id == null)
                    continue;
                var source = elem.Find("Clip", "Source").FirstOrDefault();
                if (source == null)
                    continue;
                var sourceId = source.GetAttribute("ObjectRef");

                VideoSequenceSource videoSource;
                if (!videoSequenceSources.TryGetValue(sourceId, out videoSource))
                    continue;

                var clip = new VideoClip
                {
                    Element = elem,
                    Source = videoSource,
                    ClipID = id,
                };
                videoSource.Clip = clip;
                videoClips[id] = clip;
            }
        }
    }

    public class VideoSequenceSource //AudioSequenceSource
    {
        public Element Element;
        public string ObjectId;
        public long OriginalDuration;
        public SequenceSourceCategory Category;
        public VideoClip Clip;
        public Sequence Sequence;
    }

    public class VideoClip //AudioClip
    {
        public Element Element;
        public string ClipID;
//        public MasterClip MasterClip;
        public VideoSequenceSource Source;
        public SubClip SubClip;
    }

//    public class MasterClip
//    {
//        public Element Element;
//        public string ObjectUID;
//        public List<VideoClip> Clips = new List<VideoClip>();
//        public ClipProjectItem ClipProjectItem;
//    }

    public class ClipProjectItem //ObjectUID="6b2b2a27-4ef5-474a-821b-8402133dcbb6"
    {
        public Element Element;
        public string ObjectUID;
        public string Name; //??
    }

    public class VideoClipTrackItem
    {
        public Element Element;
        public string ObjectID;
        public SubClip SubClip;
        public long Start;
        public long End;
    }

    public class SubClip
    {
        public Element Element;
        public string ObjectID;
        public string Name; //file name without path
        //public MasterClip MasterClip;
        //public string MasterClipId;
        public VideoClip Clip;
        public VideoClipTrackItem VideoClipTrackItem;
    }


    public enum SequenceSourceCategory
    {
        Video,
        Audio
    }
}
