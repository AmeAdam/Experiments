using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MyXml.PrProj
{
    public class VideoFilterComponent : BasePremiereObject
    {
        public string Name;
        public XElement Component;
        public XElement Bypass;

        public void SetEnabledState(bool enabled)
        {
            if (Bypass != null)
            {
                Bypass.Value = enabled ? "true" : "false";
            }
        }

        public IEnumerable<int> GetParamsRefs()
        {
            return Component
                .Elements("Params")
                .Elements("Param")
                .Select(e => (int)e.Attribute("ObjectRef"));
        }
    }
}