using System;
using System.Xml.Serialization;

namespace AmeCommon.CardsCapture
{
    [XmlRoot("ame-card", Namespace = "http://kamerzysta.bydgoszcz.pl")]
    [Serializable]
    public class AmeCardSettings
    {
        [XmlElement("id")]
        public string Id { get; set; }
    }
}
