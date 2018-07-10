using System;

namespace EmitMapper.Benchmarks.TestData.Automapper
{
    public class ModelObject
    {
        public DateTime BaseDate { get; set; }
        public ModelSubObject Sub { get; set; }
        public ModelSubObject Sub2 { get; set; }
        public ModelSubObject SubWithExtraName { get; set; }
    }
}