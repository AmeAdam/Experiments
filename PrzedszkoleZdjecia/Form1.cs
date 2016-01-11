using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var rootPath = @"F:\temp\wrzesien";
            Directory.CreateDirectory(rootPath);

//            foreach (HtmlElement a in webBrowser1.Document.GetElementsByTagName("a"))
  //          {

            var list = new List<string>();
            foreach (HtmlElement a in webBrowser1.Document.GetElementsByTagName("a"))
                list.Add(a.GetAttribute("href"));
        
            Parallel.ForEach(list, href => 
            {
                if (href.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase))
                {
                    var req = WebRequest.Create(href);
                    var jpgName = Path.Combine(rootPath, Path.GetFileName(href));

                    using (var response = req.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            using (var file = File.OpenWrite(jpgName))
                            {
                                stream.CopyTo(file);
                            }
                        }
                    }
                }
            });
            MessageBox.Show("complete");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://www.przedszkolnachatka.pl/wrzesien-2014_rok-2014-2015_galeria_przedszkole");
        }
    }
}
