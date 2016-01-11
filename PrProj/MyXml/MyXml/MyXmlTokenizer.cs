using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyXml.MyXml
{
    public class MyXmlTokenizer
    {
        private string buff = "";
        private bool eof;
        private readonly StringBuilder textToken = new StringBuilder();
        private readonly Stream sr;

        public MyXmlTokenizer(Stream sr)
        {
            this.sr = sr;
        }

        private readonly HashSet<char> specialChars = new HashSet<char>{'<', '>', '=', '\"'};

        public List<Token> Tokens = new List<Token>
            {
                new Token{Enum = TokenEnum.OpenComment, Value = "<!--" },
                new Token{Enum = TokenEnum.CloseComment, Value = "-->" },
                new Token{Enum = TokenEnum.OpenInstr, Value = "<?" },
                new Token{Enum = TokenEnum.CloseInstr, Value = "?>" },
                new Token{Enum = TokenEnum.OpenEndElement, Value = "</" },
                new Token{Enum = TokenEnum.CloseEndElement, Value = "/>" },
                new Token{Enum = TokenEnum.CloseBracket, Value = ">" },
                new Token{Enum = TokenEnum.OpenBracket, Value = "<" },
                new Token{Enum = TokenEnum.Equal, Value = "=" },
                new Token{Enum = TokenEnum.Quota, Value = "\"" },
            };


        private void AcceptChars(int len)
        {
            buff = buff.Substring(len);
        }

        private void UpdateBuff()
        {
            if (eof || buff.Length > 4)
                return;
            var byteBuff = new byte[50];
            int buffPos = 0;
            int count = 50;

            while (count > 0 && !eof)
            {
                int bytesRead = sr.Read(byteBuff, buffPos, count);
                if (bytesRead == 0)
                    eof = true;
                buffPos += bytesRead;
                count -= bytesRead;
            }

            if (buffPos < 50)
            {
                var newBuff = new byte[buffPos];
                Array.Copy(byteBuff, 0, newBuff, 0, buffPos);
                byteBuff = newBuff;
            }
            buff += Encoding.UTF8.GetString(byteBuff);
        }

        private bool Eof
        {
            get { return eof && buff.Length == 0; }
        }

        private Token FindToken(string str)
        {
            foreach (var token in Tokens)
            {
                if (str.StartsWith(token.Value))
                    return token;
            }
            return null;
        }

        private Token GetTextToken()
        {
            if (textToken.Length == 0)
                return null;
            var res = new Token {Enum = TokenEnum.Text, Value = textToken.ToString()};
            textToken.Clear();
            return res;
        }

        public IEnumerable<Token> Parse()
        {
            UpdateBuff();
            while (!Eof)
            {
                var token = FindToken(buff);
                if (token != null)
                {
                    var txt = GetTextToken();
                    if (txt != null)
                        yield return txt;
                    AcceptChars(token.Value.Length);
                    yield return token;
                }
                else
                {
                    if (Char.IsWhiteSpace(buff[0]))
                    {
                        var txt = GetTextToken();
                        if (txt != null)
                            yield return txt;

                        var whiteSpaces = new StringBuilder();
                        foreach (var c in buff)
                        {
                            if (Char.IsWhiteSpace(c))
                                whiteSpaces.Append(c);
                            else
                                break;
                        }
                        AcceptChars(whiteSpaces.Length);
                        yield return new Token { Enum = TokenEnum.WhiteSpace, Value = whiteSpaces .ToString()};
                    }
                    else
                    {
                        textToken.Append(buff[0]);
                        int i = 1;
                        for (; i < buff.Length; i++)
                        {
                            if (!specialChars.Contains(buff[i]) && !Char.IsWhiteSpace(buff[i]))
                                textToken.Append(buff[i]);
                            else
                                break;
                        }
                        AcceptChars(i);
                    }
                }
                UpdateBuff();
            }
            var txtToken = GetTextToken();
            if (txtToken != null)
                yield return txtToken;

            yield return new Token { Enum = TokenEnum.Eof};
        }
    }

    public class Token
    {
        public TokenEnum Enum;
        public string Value;

        public override string ToString()
        {
            return string.Format("Token: {0} Value: {1}", Enum, Value);
        }
    }

    public enum TokenEnum
    {
        OpenBracket,
        CloseBracket,
        OpenComment,
        CloseComment,
        OpenInstr,
        CloseInstr,
        Quota,
        Text,
        WhiteSpace,
        OpenEndElement,
        CloseEndElement,
        Equal,
        Eof
    }
}
