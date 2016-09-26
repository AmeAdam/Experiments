using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmeCommon.Model;
using LiteDB;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class DatabaseTest
    {
        [Test]
        public void PopulateDatabase()
        {
            var db = new LiteDatabase("C:\\Dysk Google\\ame-projekty.db");
            //var knownProjects = db.GetCollection<AmeFotoVideoProject>("projects");
            var devices = db.GetCollection<Device>("devices");
          
            foreach (var device in devices.FindAll())
            {
                Console.WriteLine(device.UniqueName);
            }


            //knownProjects.Insert(new AmeFotoVideoProject
            //{
            //    Name = "Abc",
            //    EventDate = new DateTime(2016, 09, 15),
            //    LocalPathRoot = "e:\\projekty\\2016-09-15 abc",
            //    MediaFiles = new List<MediaFile>()
            //}); knownProjects.Insert(new AmeFotoVideoProject
            //{
            //    Name = "Def",
            //    EventDate = new DateTime(2016, 09,17),
            //    LocalPathRoot = "e:\\projekty\\2016-09-15 def",
            //    MediaFiles = new List<MediaFile>()
            //});
            db.Dispose();
        }
    }
}
