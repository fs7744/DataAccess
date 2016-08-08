using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace VIC.DataAccess.Config
{
    public class DbSql
    {
        [XmlAttribute]
        public string CommandName { get; set; }

        internal string ConnectionString { get; set; }

        public string Text { get; set; }

        [XmlAttribute]
        public CommandType Type { get; set; }

        public List<Parameter> PreParameters { get; set; }

        [XmlAttribute]
        public int Timeout { get; set; }

        [XmlAttribute]
        public string ConnectionName { get; set; }
    }
}