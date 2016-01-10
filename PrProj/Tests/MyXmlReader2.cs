using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Tests
{
    [TestFixture]
    public class MyXmlReader2
    {
        [Test]
        public void ReadXml()
        {
            XDocument doc = XDocument.Load("D:\\temp\\test.prproj");

            doc.Root.SetAttributeValue("abc", "1");
            doc.Save("D:\\temp\\test2.prproj");

            //File.Delete("d:\\temp\\parsed.xml");
            //var outFile = new StreamWriter(File.OpenWrite("d:\\temp\\parsed.xml"));
            //using(var reader = XmlReader.Create("D:\\temp\\test.prproj"))
            //{
            //    while(reader.Read())
            //    {
            //        switch(reader.NodeType)
            //        {
            //            case XmlNodeType.XmlDeclaration:
            //                outFile.Write("<?"+ reader.Name +" "+reader.Value + "?>");
            //                break;
            //            case XmlNodeType.Whitespace:
            //                outFile.Write(reader.Value);
            //                break;
            //            case XmlNodeType.Element:
            //                outFile.Write("<"+reader.LocalName);
            //                reader.

            //                break;
            //            default:
            //                outFile.Write(reader.Value);
            //                break;
            //        }
            //    }
            //}
            //outFile.Dispose();
        }

    }
}
