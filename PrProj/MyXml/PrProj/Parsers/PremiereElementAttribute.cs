using System;

namespace MyXml.PrProj.Parsers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PremiereElementAttribute : Attribute
    {
        private readonly string _elementName;

        public PremiereElementAttribute(string elementName)
        {
            _elementName = elementName;
        }

        public string ElementName
        {
            get { return _elementName; }
        }
    }
}
