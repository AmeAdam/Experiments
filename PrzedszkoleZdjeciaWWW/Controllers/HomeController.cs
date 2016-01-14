using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using Microsoft.Build.Framework.XamlTypes;

namespace PrzedszkoleZdjeciaWWW.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var wc = new WebClient();
            var content = wc.DownloadData("http://www.przedszkolnachatka.pl/rok-2015-2016_galeria_przedszkole");
            HtmlDocument doc = new HtmlDocument();
            doc.Load(new MemoryStream(content), Encoding.UTF8);

            var mainDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(d => d.GetAttributeValue("style", "") == "text-align: center;");

            ViewBag.List = mainDiv.Descendants("a").Select(ParseLink).ToList();
            return View();
        }

        private Tuple<string,string> ParseLink(HtmlNode a)
        {
            var link = a.GetAttributeValue("href", "");
            var text = a.Element("span").InnerText;
            return new Tuple<string, string> (link, text);
        }

        public ActionResult Galeria(string link)
        {
            var wc = new WebClient();
            var content = wc.DownloadData(link);
            HtmlDocument doc = new HtmlDocument();
            doc.Load(new MemoryStream(content), Encoding.UTF8);

            foreach (var div in
                    doc.DocumentNode.Descendants("div")
                        .Where(d => d.GetAttributeValue("class", "") == "image-gallery")
                        .ToList())
            {
                div.SetAttributeValue("class", "");
                var a = div.Element("a");
                var href = a.GetAttributeValue("href", "");
                a.Remove();
                var img = doc.CreateElement("img");
                img.SetAttributeValue("src", href);
                img.SetAttributeValue("width", "150");
                div.AppendChild(img);
            }

            return Content(doc.DocumentNode.OuterHtml, "text/html", Encoding.UTF8);
            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}