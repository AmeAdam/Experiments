using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class WatcherTest
    {
        [Test]
        public void Watch()
        {
            FileSystemWatcher w = new FileSystemWatcher(@"E:\Projekty\2016-10-16 Nazwa", "*");
            w.IncludeSubdirectories = true;
            w.Changed += Cahcned;
            w.Deleted += Cahcned;
            w.Renamed += Cahcned;
            w.Created += Cahcned;
            w.EnableRaisingEvents = true;
            Thread.Sleep(1000000);
        }

        private void Cahcned(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
        }
    }
}
