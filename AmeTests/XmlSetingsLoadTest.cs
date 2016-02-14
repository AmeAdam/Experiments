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
using CardGrabberCmd.MediaTasks.Settings;
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
            XmlSerializer xs = new XmlSerializer(typeof(CardProfilesSettings));
            CardProfilesSettings temp;
            using (var xr = doc.CreateReader())
            {
                temp = (CardProfilesSettings) xs.Deserialize(xr);
            }
            
        }
    }
}
