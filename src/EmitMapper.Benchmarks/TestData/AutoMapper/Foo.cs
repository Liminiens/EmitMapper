using System;
using System.Collections.Generic;

namespace EmitMapper.Benchmarks.TestData.Automapper
{
    public class Foo
    {
        public string Name { get; set; }

        public int Int32 { get; set; }

        public long Int64 { set; get; }

        public int? NullInt { get; set; }

        public float Floatn { get; set; }

        public double Doublen { get; set; }

        public DateTime DateTime { get; set; }

        public InnerFoo Foo1 { get; set; }

        public List<InnerFoo> Foos { get; set; }

        public InnerFoo[] FooArr { get; set; }

        public int[] IntArr { get; set; }

        public int[] Ints { get; set; }
    }
}