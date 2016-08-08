using System.Data;
using System.Xml.Serialization;

namespace VIC.DataAccess.Config
{
    public class Parameter
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        [XmlAttribute]
        public DbType Type { get; set; }

        [XmlAttribute]
        public int Size { get; set; }

        [XmlAttribute]
        public bool IsNullable { get; set; } = true;

        [XmlAttribute]
        public byte Precision { get; set; }

        [XmlAttribute]
        public byte Scale { get; set; }

        internal DataParameter ToDataParameter()
        {
            return new DataParameter()
            {
                DbType = Type,
                Direction = Direction,
                IsNullable = IsNullable,
                ParameterName = Name,
                Precision = Precision,
                Scale = Scale,
                Size = Size
            };
        }
    }
}