using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CardGrabberCmd;
using AmeCommon.MediaTasks.Settings;
using NUnit.Framework.Internal;
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
            XmlSerializer xs = new XmlSerializer(typeof(AmeSettings));
            AmeSettings temp;
            using (var xr = doc.CreateReader())
            {
                temp = (AmeSettings) xs.Deserialize(xr);
            }
            
        }
    }
}
