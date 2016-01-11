using System.IO;

namespace MyXml.MyXml
{
    public class TextNode : BaseNode
    {
        public string Content;

        public TextNode(string content)
        {
            Content = content;
        }

        public override void Write(StreamWriter sw)
        {
            sw.Write(Content);
        }
    }
}