using System.IO;
using MyXml.MyXml;
using MyXml.PrProj;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class ProjReaderTest
    {
        [Test]
        public void ReadeProj()
        {
            var doc = new MyXmlDoc();
            using (var file = File.OpenRead("resources/proj1.prproj.xml"))
            {
                doc.Load(file);
            }

            var pr = new ProjReader();
            pr.Load(doc.Root);

            Assert.AreEqual(1, pr.Sequences.Count);
            var seq = pr.Sequences.Values.FirstOrDefault();
            Assert.IsNotNull(seq);
            Assert.AreEqual("04_pierwszy_taniec", seq.Name);
            Assert.AreEqual(1, seq.Sources.Count);
            Assert.AreEqual(566039093760000, seq.Sources[0].OriginalDuration);

            var trackItem = seq.Sources[0].Clip.SubClip.VideoClipTrackItem;
            Assert.AreEqual(516089387520000, trackItem.End);
            Assert.AreEqual(1798433280000, trackItem.Start);

        }
    }
}
