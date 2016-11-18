using System;
using AmeCommon.VideoMonitoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GiadaServer.Controllers
{
    public class HomeController : Controller
    {
        private IVideoGrabber videoGrabber;
        private readonly IMemoryCache memCache;

        public HomeController(IVideoGrabber videoGrabber, IMemoryCache memCache)
        {
            this.videoGrabber = videoGrabber;
            this.memCache = memCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Camera()
        {
            var img = videoGrabber.GetCameraViewJpg();
            return File(img, "image/jpeg", DateTime.UtcNow.Ticks + ".jpg");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult CameraView()
        {
            return View();
        }
    }
}
