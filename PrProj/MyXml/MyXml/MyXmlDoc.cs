using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyXml.MyXml
{
    public class MyXmlDoc
    {
        public Element Root = new Element
            {
                Name = "Root",
                Childs = new List<BaseNode>()
            };

        public void Write(Stream str)
        {
            using (var sw = new StreamWriter(str))
            {
                foreach (var child in Root.Childs)
                    child.Write(sw);
            }
        }

        public void Load(Stream str)
        {
            var tokenizer = new MyXmlTokenizer(str);
            var enumerator = tokenizer.Parse().GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ApplicationException();

            while (enumerator.Current.Enum != TokenEnum.Eof)
            {
                switch (enumerator.Current.Enum)
                {
                    case TokenEnum.Text:
                        Root.Childs.Add(new TextNode(enumerator.Current.Value));
                        enumerator.MoveNext();
                        break;
                    case TokenEnum.WhiteSpace:
                        Root.Childs.Add(new TextNode(enumerator.Current.Value));
                        enumerator.MoveNext();
                        break;
                    case TokenEnum.OpenInstr:
                        Root.Childs.Add(ProcessingInstr.Parse(enumerator));
                        break;
                    case TokenEnum.OpenComment:
                        Root.Childs.Add(Comment.Parse(enumerator));
                        break;
                    case TokenEnum.OpenBracket:
                        Root.Childs.Add(Element.Parse(enumerator, Root));
                        break;
                    default:
                        throw new ApplicationException("Unexpected token: "+enumerator.Current);
                }
            }
        }

        IEnumerable<Element> GetElementsByTagName(string tag, Element parent)
        {
            if (parent.Name == tag)
                yield return parent;
            foreach (var child in parent.Childs.OfType<Element>())
            {
                foreach (var founded in GetElementsByTagName(tag, child))
                    yield return founded;
            }
        }

        public IEnumerable<Element> GetElementsByTagName(string tag)
        {
            foreach (var child in Root.Childs.OfType<Element>())
            {
                foreach (var res in GetElementsByTagName(tag, child))
                    yield return res;
            }
        }

        public void RemoveElement(Element elem)
        {
            RemoveElement(Root.Childs, elem);
        }

        private bool RemoveElement(List<BaseNode> list, Element elemToRemove)
        {
            if (list == null || list.Count == 0)
                return false;
            if (list.Remove(elemToRemove))
                return true;
            foreach(var child in list.OfType<Element>())
            {
                if (RemoveElement(child.Childs, elemToRemove))
                    return true;
            }
            return false;
        }
    }
}
