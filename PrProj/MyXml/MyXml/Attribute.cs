using System;
using System.Collections.Generic;
using System.Text;

namespace MyXml.MyXml
{
    public class Attribute
    {
        public string Name;
        public string Value;

        public static Attribute Parse(IEnumerator<Token> enumerator)
        {
            var res = new Attribute {Name = enumerator.Current.Value};
            SkipWhiteSpace(enumerator);
            if (enumerator.Current.Enum != TokenEnum.Equal)
                throw new ApplicationException();
            SkipWhiteSpace(enumerator);
            if (enumerator.Current.Enum != TokenEnum.Quota)
                throw new ApplicationException();
            var sb = new StringBuilder();
            var next = Next(enumerator);
            while (next.Enum != TokenEnum.Quota)
            {
                sb.Append(next.Value);
                next = Next(enumerator);
            }
            res.Value = sb.ToString();
            return res;
        }

        private static void SkipWhiteSpace(IEnumerator<Token> enumerator)
        {
            var next = Next(enumerator);
            while (enumerator.Current.Enum == TokenEnum.WhiteSpace)
                Next(enumerator);
        }

        private static Token Next(IEnumerator<Token> enumerator)
        {
            if (!enumerator.MoveNext())
                throw new ApplicationException("Unexpected end of XML");
            return enumerator.Current;
        }
    }
}