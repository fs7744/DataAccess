using System;

namespace MSSqlExample
{
    public class Student
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime? JoinDate { get; set; }

        public decimal? Money { get; set; }
    }
}