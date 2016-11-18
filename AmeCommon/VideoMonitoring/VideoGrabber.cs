using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Ozeki.Camera;
using Ozeki.Media;

namespace AmeCommon.VideoMonitoring
{
    public interface IVideoGrabber
    {
        byte[] GetCameraViewJpg();
    }

    public class VideoGrabber : IVideoGrabber, IDisposable
    {
        //DiscoverCameras _discover;
        private readonly BitmapSourceProvider imageProvider = new BitmapSourceProvider();
        private readonly MediaConnector connector = new MediaConnector();
        private readonly SnapshotHandler snapshotHandler = new SnapshotHandler();
        private ICamera camera;
        private readonly DiscoverCameras discover;
        private List<ICamera> avaliableCameras = new List<ICamera>();

        public VideoGrabber()
        {
            discover = new DiscoverCameras();
            discover.Discovered += CameraDiscovered;
            discover.DiscoverWebCameras();
            //camera = new OzekiCamera("usb://DeviceId=0;Name=Microsoft® LifeCam HD-5000;");
        }

        private void CameraDiscovered(object sender, DiscoveredDeviceArgs discoveredCam)
        {
            camera = discoveredCam.Camera;
            connector.Connect(camera.VideoChannel, imageProvider);
            connector.Connect(camera.VideoChannel, snapshotHandler);
            camera.Start();
        }

        public List<string> GetAllCameras()
        {
            return null;
        }

        public byte[] GetCameraViewJpg()
        {
            var sn = snapshotHandler.TakeSnapshot();
            var img = sn.ToImage();
            var mem = new MemoryStream();
            img.Save(mem, ImageFormat.Jpeg);
            img.Dispose();
            return mem.ToArray();
        }

        public void Dispose()
        {
            camera.Stop();
            connector.Disconnect(camera.VideoChannel, imageProvider);
            connector.Disconnect(camera.VideoChannel, snapshotHandler);
        }
    }

    public class Camera
    {
        private readonly BitmapSourceProvider imageProvider = new BitmapSourceProvider();
        private readonly MediaConnector connector = new MediaConnector();
        private readonly SnapshotHandler snapshotHandler = new SnapshotHandler();
        private readonly ICamera camera;

        public Camera(ICamera camera)
        {
            this.camera = camera;
        }

        public void Start()
        {
            connector.Connect(camera.VideoChannel, imageProvider);
            connector.Connect(camera.VideoChannel, snapshotHandler);
            camera.Start();
        }

        public byte[] GetCameraViewJpg()
        {
            var sn = snapshotHandler.TakeSnapshot();
            var img = sn.ToImage();
            var mem = new MemoryStream();
            img.Save(mem, ImageFormat.Jpeg);
            img.Dispose();
            return mem.ToArray();
        }

        public void Stop()
        {
            camera.Stop();
            connector.Disconnect(camera.VideoChannel, imageProvider);
            connector.Disconnect(camera.VideoChannel, snapshotHandler);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
