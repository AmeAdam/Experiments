using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyXml.MyXml
{
    public class Element : BaseNode
    {
        public string Name;
        public List<Attribute> Attributes = new List<Attribute>();
        public List<BaseNode> Childs = new List<BaseNode>();
        public bool ShortClose;
        public Element Parent;

        public override string ToString()
        {
            return Name ?? base.ToString();
        }

        public string InnerText
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var txt in Childs.OfType<TextNode>())
                    sb.Append(txt.Content);
                return sb.ToString();
            }
            set
            {
                var textNodes = Childs.OfType<TextNode>().ToList();
                if (textNodes.Count == 1)
                {
                    textNodes.First().Content = value;
                }
                else
                {
                    Childs.RemoveAll(c => c is TextNode);
                    Childs.Add(new TextNode(value));
                }
            }
        }

        public void Show(int indent)
        {
            Console.Write(new string(' ', indent));
            Console.WriteLine("Element "+ Name);
            foreach (var child in Childs)
            {
                var e = child as Element;
                if (e!=null)
                    e.Show(indent+3);
            }
        }

        public IEnumerable<Element> Find(params string[] path)
        {
            return Find(new List<string>(path));
        }

        private IEnumerable<Element> Find(List<string> path)
        {
            foreach (var child in Childs.OfType<Element>().Where(e=>e.Name == path[0]))
            {
                var innerPath = path.GetRange(1, path.Count - 1);
                if (innerPath.Count > 0)
                {
                    foreach (var childChild in child.Find(innerPath))
                        yield return childChild;
                }
                else
                    yield return child;
            }
        }

        public Element GetElement(string name)
        {
            return GetElements(name).FirstOrDefault();
        }

        public IEnumerable<Element> GetElements(string name)
        {
            return Childs.OfType<Element>().Where(c => c.Name == name);
        }



        public override void Write(StreamWriter sw)
        {
            sw.Write("<");
            sw.Write(Name);
            if (Attributes.Any())
            {
                foreach (var attr in Attributes)
                {
                    sw.Write(" ");
                    sw.Write(attr.Name);
                    sw.Write("=\"");
                    sw.Write(attr.Value);
                    sw.Write("\"");
                }
            }

            if (Childs.Any())
            {
                sw.Write(">");
                foreach (var child in Childs)
                    child.Write(sw);
                sw.Write("</" + Name + ">");
            }
            else
                sw.Write("/>");
        }

        public static BaseNode Parse(IEnumerator<Token> enumerator, Element parent)
        {
            if (!enumerator.MoveNext())
                throw new ApplicationException("Unexpected end of XML");
            if (enumerator.Current.Enum != TokenEnum.Text)
                throw new ApplicationException("Unexpected value");
            var res = new Element();
            res.Parent = parent;
            res.Name = enumerator.Current.Value;
            SkipWhiteSpace(enumerator);

            switch (enumerator.Current.Enum)
            {
                case TokenEnum.CloseEndElement:
                    return res;
                case TokenEnum.CloseBracket:
                    enumerator.MoveNext();
                    res.Childs.Add(new TextNode(""));
                    ProcessElementContent(enumerator, res);
                    return res;
                case TokenEnum.Text:
                    ReadAttributes(enumerator, res);
                    if (enumerator.Current.Enum == TokenEnum.CloseEndElement)
                    {
                        enumerator.MoveNext();
                        return res;
                    }
                    if (enumerator.Current.Enum == TokenEnum.CloseBracket)
                    {
                        enumerator.MoveNext();
                        ProcessElementContent(enumerator, res);
                        return res;
                    }
                    throw new ApplicationException();
                default:
                    throw new ApplicationException("Unexpected");
            }
        }

        private static void ProcessElementContent(IEnumerator<Token> enumerator, Element parent)
        {
            while (enumerator.Current.Enum != TokenEnum.Eof)
            {
                switch (enumerator.Current.Enum)
                {
                    case TokenEnum.Text:
                        parent.Childs.Add(new TextNode(enumerator.Current.Value));
                        enumerator.MoveNext();
                        break;
                    case TokenEnum.WhiteSpace:
                        parent.Childs.Add(new TextNode(enumerator.Current.Value));
                        enumerator.MoveNext();
                        break;
                    case TokenEnum.Quota:
                        parent.Childs.Add(new TextNode(enumerator.Current.Value));
                        enumerator.MoveNext();
                        break;
                    case TokenEnum.Equal:
                        parent.Childs.Add(new TextNode(enumerator.Current.Value));
                        enumerator.MoveNext();
                        break;
                    case TokenEnum.OpenInstr:
                        parent.Childs.Add(ProcessingInstr.Parse(enumerator));
                        break;
                    case TokenEnum.OpenComment:
                        parent.Childs.Add(Comment.Parse(enumerator));
                        break;
                    case TokenEnum.OpenBracket:
                        parent.Childs.Add(Parse(enumerator, parent));
                        break;
                    case TokenEnum.OpenEndElement:
                        SkipWhiteSpace(enumerator);
                        if (!(enumerator.Current.Enum == TokenEnum.Text && enumerator.Current.Value == parent.Name))
                            throw new ApplicationException();
                        SkipWhiteSpace(enumerator);
                        if (enumerator.Current.Enum != TokenEnum.CloseBracket)
                            throw new ApplicationException();
                        enumerator.MoveNext();
                        return;
                    case TokenEnum.CloseEndElement:
                        return;
                    default:
                        throw new ApplicationException("Unexpected token: " + enumerator.Current);
                }
            }
        }

        private static bool IsCloseElement(IEnumerator<Token> enumerator)
        {
            return enumerator.Current.Enum == TokenEnum.CloseEndElement || enumerator.Current.Enum == TokenEnum.CloseBracket;
        }

        private static void ReadAttributes(IEnumerator<Token> enumerator, Element res)
        {
            while (!IsCloseElement(enumerator))
            {
                res.Attributes.Add(Attribute.Parse(enumerator));
                SkipWhiteSpace(enumerator);
            }
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

        public string GetAttribute(string attrName)
        {
            var attr = Attributes.Find(a => a.Name == attrName);
            if (attr == null)
                return null;
            return attr.Value;
        }

        public void SetAttribute(string attrName, string value)
        {
            var attr = Attributes.Find(a => a.Name == attrName);
            if (attr == null)
                return;
            attr.Value = value;
        }

        public string GetChildNodeValue(string nodeName)
        {
            var child = Find(nodeName).FirstOrDefault();
            if (child == null)
                return null;
            return child.InnerText;
        }

        public void SetChildNodeValue(string nodeName, string value)
        {
            var child = Find(nodeName).FirstOrDefault();
            if (child == null)
                return;
            child.InnerText = value;
        }

        public string ChildNodeAttrValue(string nodeName, string attrName)
        {
            var child = Find(nodeName).FirstOrDefault();
            if (child == null)
                return null;
            return child.GetAttribute(attrName);
        }

        public void RemoveFromParent()
        {
            if (Parent == null)
                return;
            var pos = Parent.Childs.IndexOf(this);
            Parent.Childs.RemoveAt(pos);
            var next = Parent.Childs[pos] as TextNode;
            if (next != null && next.Content.All(Char.IsWhiteSpace))
                Parent.Childs.RemoveAt(pos);
        }
    }
}