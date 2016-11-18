using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Ozeki.Camera;
using Ozeki.Media;

namespace AmeTests
{
    [TestFixture]
    public class CaneraGetPhoto
    {
        [Test]
        public void RunCamera()
        {
            //DiscoverCameras _discover;
            var imageProvider = new BitmapSourceProvider();
            var mediaConnector = new MediaConnector();
            SnapshotHandler sh = new SnapshotHandler();


            var webCamera = new OzekiCamera("usb://DeviceId=0;Name=Microsoft® LifeCam HD-5000;");

            var ok = mediaConnector.Connect(webCamera.VideoChannel, imageProvider);
            ok = mediaConnector.Connect(webCamera.VideoChannel, sh);
            webCamera.Start();

            Thread.Sleep(3000);
            var sn = sh.TakeSnapshot();
            var img = sn.ToImage();
            img.Save("d:\\cam.jpg", ImageFormat.Jpeg);

            webCamera.Stop();

        }
    }
}
