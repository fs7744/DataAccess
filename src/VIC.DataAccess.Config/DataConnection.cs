using System.Xml.Serialization;

namespace VIC.DataAccess.Config
{
    public class DataConnection
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ConnectionString { get; set; }
    }
}