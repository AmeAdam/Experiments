using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyXml.MyXml
{
    public class Comment : BaseNode
    {
        public string Content;
        public static Comment Parse(IEnumerator<Token> tokens)
        {
            var res = new Comment();
            var sb = new StringBuilder();
            sb.Append(tokens.Current.Value);
            while (tokens.MoveNext())
            {
                sb.Append(tokens.Current.Value);
                if (tokens.Current.Enum == TokenEnum.CloseComment)
                {
                    tokens.MoveNext();
                    break;
                }
            }
            res.Content = sb.ToString();
            return res;
        }

        public override void Write(StreamWriter sw)
        {
            sw.Write(Content);
        }
    }
}