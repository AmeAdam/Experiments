using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyXml.MyXml
{
    public class ProcessingInstr : BaseNode
    {
        public string Content;
        public static ProcessingInstr Parse(IEnumerator<Token> tokens)
        {
            var res = new ProcessingInstr();
            var sb = new StringBuilder();
            sb.Append(tokens.Current.Value);
            while (tokens.MoveNext())
            {
                sb.Append(tokens.Current.Value);
                if (tokens.Current.Enum == TokenEnum.CloseInstr)
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