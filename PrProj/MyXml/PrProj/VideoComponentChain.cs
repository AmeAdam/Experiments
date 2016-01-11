using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyXml.MyXml;
using System.Xml.Linq;

namespace MyXml.PrProj
{
    public class VideoComponentChain : BasePremiereObject
    {
        public int SeqId;
        public XElement componentsRoot;
        public List<Component> components;

        private Component GetComponentWithName(string name)
        {
            if (components == null)
                return null;
            return components.FirstOrDefault(c => c.ReferencedComponent != null && c.ReferencedComponent.Name == name);
        }

        //aby element by³ wizualnie pierwszy musi byæ na liœcie jako ostatni
        public void SetEffectTop(string effectName)
        {
            var topEffect = GetComponentWithName(effectName);
            if (topEffect == null)
                return;

            var pos = components.IndexOf(topEffect);
            while (pos < components.Count - 1)
            {
                var aboveElement = components[pos + 1];
                aboveElement.SwitchData(topEffect);
                topEffect = aboveElement;
                pos++;
            }
        }

        public void RemoveEffect(string effectName)
        {
            var effectToRemove = GetComponentWithName(effectName);
            if (effectToRemove == null || effectToRemove.ReferencedComponent == null)
                return;
            components.Remove(effectToRemove);
            effectToRemove.Element.Remove();

            ReindexComponents();

            var videoFilterComponentElement = effectToRemove.ReferencedComponent.Element;
            videoFilterComponentElement.Remove();

            var paramDict = Element.Parent.Elements("VideoComponentParam").ToDictionary(elem => (int)elem.Attribute("ObjectID"));
            foreach (var paramRef in effectToRemove.ReferencedComponent.GetParamsRefs())
            {
                XElement param;
                if (paramDict.TryGetValue(paramRef, out param))
                    param.Remove();
            }
        }


        public void SetEnabledState(string effectName, bool enabled)
        {
            var component = GetComponentWithName(effectName);
            if (component == null || component.ReferencedComponent == null)
                return;
            component.ReferencedComponent.SetEnabledState(enabled);
        }

        private void ReindexComponents()
        {
            var index = 0;
            foreach (var cmp in components)
                cmp.Index = index++;
        }

        public class Component
        {
            public string ObjectRef;
            public XElement Element;
            public VideoFilterComponent ReferencedComponent;
            private int _index;

            public Component(int index)
            {
                _index = index;
            }

            public int Index
            {
                get { return _index; }

                set
                {
                    Element.SetAttributeValue("Index", value.ToString(CultureInfo.InvariantCulture));
                    _index = value;
                }
            }

            public void SwitchData(Component other)
            {
                var objectRef = other.ObjectRef;
                var referencedComponent = other.ReferencedComponent;

                other.ObjectRef = ObjectRef;
                other.ReferencedComponent = ReferencedComponent;
                other.Element.SetAttributeValue("ObjectRef", other.ObjectRef);

                ObjectRef = objectRef;
                ReferencedComponent = referencedComponent;
                Element.SetAttributeValue("ObjectRef", ObjectRef);
            }
        }

    }
}