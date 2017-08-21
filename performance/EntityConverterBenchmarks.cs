using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using System;
using System.Collections.Generic;
using Vic.Data;
using Vic.Data.Abstraction;

namespace performance
{
    public class People
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public string CanNotReadName { private get; set; }

        public string FiledName;

        public DateTime DateTime { get; set; }

        public long Long { get; set; }

        public decimal Decimal { get; set; }

        public double Double { get; set; }

        public float Float { get; set; }

        public short Short { get; set; }

        public byte Byte { get; set; }

        public Guid Guid { get; set; }

        public bool Bool { get; set; }
    }

    public class Student : People
    {
        public DateTime DateTime2 { get; set; }

        //public int? ClassNumber { get; set; }

        //public long? Long2 { get; set; }

        //public decimal? Decimal2 { get; set; }

        public double Double2 { get; set; }

        public float Float2 { get; set; }

        public short Short2 { get; set; }

        public byte Byte2 { get; set; }

        public Guid Guid2 { get; set; }

        public bool Bool2 { get; set; }
    }

    [AllStatisticsColumn]
    [MemoryDiagnoser]
    public class EntityConverterBenchmarks
    {
        private List<Student> _Students = new List<Student>()
        {
            new Student()
            {
                Age = 1,
                Name = "Victor1",
                //ClassNumber = 2,
                Long = 3L,
                Decimal = 4M,
                Byte = 2,
                DateTime = new DateTime(1990,2,3),
                Double = 4.4D,
                Float = 33.3F,
                Short = 77,
                Guid = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
                Bool = true
            },
            new Student() { Age = 2, Name = "Victor2" },
            new Student()
            {
                Age = 3,
                Name = "Victor3",
                //ClassNumber = 2,
                //Long2 = 3L,
                //Decimal2 = 4M,
                Byte2 = 2,
                DateTime2 = new DateTime(1990,2,3),
                Double2 = 4.4D,
                Float2 = 33.3F,
                Short2 = 77,
                Guid2 = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
                Bool2 = false
            },
        };

        private readonly ListDataReader<Student> reader;
        private readonly EmitEntityConverter<Student> _EmitEntityConverter;
        //private readonly ExpressionEntityConverter<Student> _ExpressionEntityConverter;
        //private readonly IDbValueGetConverter dbValueGetConverter = new DbValueGetConverter();

        public EntityConverterBenchmarks()
        {
            reader = new ListDataReader<Student>(_Students);
            _EmitEntityConverter = new EmitEntityConverter<Student>();
            //_ExpressionEntityConverter = new ExpressionEntityConverter<Student>(dbValueGetConverter);
            reader.Read();
            _EmitEntityConverter.Convert(reader);
            //_ExpressionEntityConverter.Convert(reader);
        }

        [Benchmark]
        public void EmitEntityConverter()
        {
            _EmitEntityConverter.Convert(reader);
        }

        //[Benchmark]
        //public void ExpressionEntityConverter()
        //{
        //    _ExpressionEntityConverter.Convert(reader);
        //}
    }
}