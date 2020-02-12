using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LEA_for_ED
{
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Settings
    {
        public SettingsLeaConnection LeaConnection { get; set; }
        public SettingsGameProject GameProject { get; set; }
        public SettingsEDPathInfo EDPathInfo { get; set; }
        [XmlArrayItemAttribute("StatusEvent", IsNullable = false)]
        public SettingsStatusEvent[] StatusEvents { get; set; }
    }

    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SettingsLeaConnection
    {
        public string Password { get; set; }
        public int Port { get; set; }
    }

    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SettingsGameProject
    {
        public string GameName { get; set; }
        public string ProjectName { get; set; }
    }

    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SettingsEDPathInfo
    {
        public string DataPath { get; set; }
        public string StatusFile { get; set; }
        public string JournalFile { get; set; }
    }

    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class SettingsStatusEvent
    {
        [XmlAttributeAttribute()]
        public string Event { get; set; }
        [XmlAttributeAttribute()]
        public string EMTag { get; set; }
        [XmlAttributeAttribute()]
        public string Default { get; set; }
    }
}
