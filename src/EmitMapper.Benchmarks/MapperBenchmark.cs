using System;
using System.Security.Cryptography;
using AutoFixture;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EmitMapper.Benchmarks
{
    public class AuthorModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
    }

    public class AuthorDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
    }

    [ MarkdownExporter]
    public class MapperBenchmark
    {
        [Benchmark]
        public void EmitMapper()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<AuthorModel, AuthorDTO>();
            Fixture fixture = new Fixture();

            for (int i = 0; i < 1_000_000; i++)
            {
                var source = fixture.Create<AuthorModel>();
                var result = mapper.Map(source);
                var temp = result.Address;
            }
        }

        [Benchmark]
        public void AutoMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<AuthorModel, AuthorDTO>();
            });
            Fixture fixture = new Fixture();
            var mapper = config.CreateMapper();

            for (int i = 0; i < 1_000_000; i++)
            {
                var source = fixture.Create<AuthorModel>();
                var result = mapper.Map<AuthorModel, AuthorDTO>(source);
                var temp = result.Address;
            }
        }
    }
}