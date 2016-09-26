using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class JconSerializationTest
    {
        [Test]
        public void ComplexSerializationTest()
        {
            var di = new DirectoryInfo("d:\\recovered");
            var txt = JsonConvert.SerializeObject(di, Formatting.Indented);
            var di2 = JsonConvert.DeserializeObject<DirectoryInfo>(txt);
        }
    }
}
