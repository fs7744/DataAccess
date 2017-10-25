using System;
using System.Collections;
using System.Collections.Generic;

namespace MSSqlExample
{
    public class Student
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime? JoinDate { get; set; }

        public decimal? Money { get; set; }

        public Array Array { get; set; }

        public int[] Array1 { get; set; }

        public List<int> Array2 { get; set; }

        public IEnumerable<int> Array3 { get; set; }

        public IEnumerable Array4 { get; set; }
    }
}