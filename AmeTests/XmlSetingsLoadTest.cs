using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using AmeCommon.MediaTasks.Settings;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class XmlSetingsLoadTest
    {
        [Test]
        public void LoadDocument()
        {
            var doc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cards-profiles.xml"));
            XmlSerializer xs = new XmlSerializer(typeof(CardGrabberSettings));
            CardGrabberSettings temp;
            using (var xr = doc.CreateReader())
            {
                temp = (CardGrabberSettings) xs.Deserialize(xr);
            }
            
        }
    }
}
