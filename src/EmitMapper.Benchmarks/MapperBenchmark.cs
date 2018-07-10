using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using EmitMapper.Benchmarks.TestData;
using EmitMapper.Benchmarks.TestData.Automapper;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class MapperBenchmark
    {
        private const int IterationCount = 1_000;

        private ObjectsMapper<Customer, CustomerDTO> _customerEmitMapper;
        private ObjectsMapper<BenchSource, BenchDestination> _benchSourceEmitMapper;
        private ObjectsMapper<Foo, FooDest> _fooEmitMapper;
        private ObjectsMapper<B2, A2> _simpleEmitMapper;

        private IMapper _autoMapper;

        private BenchSource _benchSource;
        private B2 _simpleSource;

        private List<B2> _simple100List;
        private List<B2> _simple1000List;
        private List<Customer> _customersSample;
        private List<Foo> _fooSample;

        [GlobalSetup]
        public void Setup()
        {
            var fixture = new Fixture();

            _benchSourceEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<BenchSource, BenchDestination>();
            _simpleEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>();
            _customerEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<Customer, CustomerDTO>();
            _fooEmitMapper = ObjectMapperManager.DefaultInstance.GetMapper<Foo, FooDest>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BenchSource, BenchDestination>();
                cfg.CreateMap<BenchSource.Int1, BenchDestination.Int1>();
                cfg.CreateMap<BenchSource.Int2, BenchDestination.Int2>();
                cfg.CreateMap<A2, B2>();

                cfg.CreateMap<Address, Address>();
                cfg.CreateMap<Address, AddressDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Foo, FooDest>();
                cfg.CreateMap<InnerFoo, InnerFooDest>();
            });
            _autoMapper = config.CreateMapper();

            _benchSource = fixture.Create<BenchSource>();
            _simpleSource = fixture.Create<B2>();
            _simple100List = fixture.CreateMany<B2>(100).ToList();
            _simple1000List = fixture.CreateMany<B2>(1000).ToList();

            _customersSample = fixture.CreateMany<Customer>(10).ToList();
            _fooSample = fixture.CreateMany<Foo>(10).ToList();
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public BenchDestination EmitMapper_BenchSource()
        {
            return _benchSourceEmitMapper.Map(_benchSource);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public BenchDestination AutoMapper_BenchSource()
        {
            return _autoMapper.Map<BenchSource, BenchDestination>(_benchSource);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public A2 EmitMapper_Simple()
        {
            return _simpleEmitMapper.Map(_simpleSource);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public A2 AutoMapper_Simple()
        {
            return _autoMapper.Map<B2, A2>(_simpleSource);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<A2> EmitMapper_SimpleList100()
        {
            return _simpleEmitMapper.MapEnum(_simple100List).ToList();
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<A2> AutoMapper_SimpleList100()
        {
            return _autoMapper.Map<List<B2>, List<A2>>(_simple100List);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<A2> EmitMapper_SimpleList1000()
        {
            return _simpleEmitMapper.MapEnum(_simple1000List).ToList();
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<A2> AutoMapper_SimpleList1000()
        {
            return _autoMapper.Map<List<B2>, List<A2>>(_simple1000List);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<CustomerDTO> EmitMapper_CustomerList10()
        {
            return _customerEmitMapper.MapEnum(_customersSample).ToList();
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<CustomerDTO> AutoMapper_CustomerList10()
        {
            return _autoMapper.Map<List<Customer>, List<CustomerDTO>>(_customersSample);
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<FooDest> EmitMapper_FooList10()
        {
            return _fooEmitMapper.MapEnum(_fooSample).ToList();
        }

        [Benchmark(OperationsPerInvoke = IterationCount)]
        public List<FooDest> AutoMapper_FooList10()
        {
            return _autoMapper.Map<List<Foo>, List<FooDest>>(_fooSample);
        }
    }
}