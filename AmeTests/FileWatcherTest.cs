using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class FileWatcherTest
    {
        [Test]
        public void FileWatcher()
        {
            MyMessageHandler h = new MyMessageHandler();
            FileSystemWatcher watcher = new FileSystemWatcher("M:", "ame.xml");
            watcher.Changed += FileChanged;
            watcher.Error += FileError;
            watcher.Disposed += FileDisposed;
            watcher.Deleted += FileDeleted;
            watcher.Renamed += FileRenamed;
            watcher.EnableRaisingEvents = true;
            Thread.Sleep(300000);
            h.ReleaseHandle();
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            
        }

        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            
        }

        private void FileDisposed(object sender, EventArgs e)
        {
            
        }

        private void FileError(object sender, ErrorEventArgs e)
        {
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
        }
    }

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public sealed class MyMessageHandler : NativeWindow
    {
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int WM_DEVICECHANGE = 0x0219;

        public MyMessageHandler()
        {
            var cp = new CreateParams();
            CreateHandle(cp);
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_DEVICECHANGE)
            {
                
            }
            else if (msg.Msg == DBT_DEVICEARRIVAL)
            {
                
            }

            base.WndProc(ref msg);
        }
    }
}
