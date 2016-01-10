using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyXml.PrProj;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PremiereProjectTest
    {
        [Test]
        public void LoadFile()
        {
            XDocument doc = XDocument.Load("resources\\proj1.prproj.xml");
            var proj = new PremiereProject(doc);
        }
    }
}
