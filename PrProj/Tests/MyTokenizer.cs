using System;
using System.IO;
using MyXml.MyXml;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class MyTokenizer
    {
        [Test]
        public void Parse()
        {
            using (var file = File.OpenRead("test.xml"))
            {
                var pars = new MyXmlTokenizer(file);
                foreach (var token in pars.Parse())
                {
                    Console.WriteLine("token: "+token.Enum+ "   value: "+token.Value);
                }
            }
        }

        [Test]
        public void DocProcessing()
        {
            using (var file = File.OpenRead(@"C:\Users\adam\Google Drive\VideoTech\PluralEyeCorrector\examples\seq1_norm111.prproj.bak"))
            {
                var doc = new MyXmlDoc();
                doc.Load(file);
                using (var outFile = File.OpenWrite("test-out.xml"))
                {
                    foreach (var e in doc.Root.Find("PremiereData", "Project", "Node"))
                    {
                        var attr = e.Attributes.Find(a => a.Name == "Version");
                        attr.Value = "4";
                    }

                    doc.Write(outFile);
                }
            }
        }
    }
}
