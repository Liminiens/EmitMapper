using System;
using System.Data;
using System.Linq;
using EmitMapper.NetStandard;
using EmitMapper.NetStandard.MappingConfiguration;
using EmitMapper.NetStandard.MappingConfiguration.MappingOperations;
using EmitMapper.NetStandard.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.NetStandard.Utils;
using Xunit;

namespace EmitMapper.Tests
{
    public class Map2DataRowConfig : MapConfigBase<Map2DataRowConfig>
    {
        public override IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            var objectMembers = ReflectionUtils.GetPublicFieldsAndProperties(from);
            return base.FilterOperations(
                from,
                to,
                objectMembers.Select(
                    m => (IMappingOperation)new SrcReadOperation
                    {
                        Source = new MemberDescriptor(m),
                        Setter = (obj, value, state) =>
                        {
                            ((DataRow)obj)[m.Name] = value ?? DBNull.Value;
                        }
                    }
                )
            ).ToArray();
        }
    }
    // Test data object
    public class TestDTO
    {
        public string field1 = "field1";
        public int field2 = 10;
        public bool field3 = true;
    }
    public class TestDTO2
    {
        public string field1;
        public int field2;
        public bool field3;
    }

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TestDTO, TestDTO2>();

            // initialization of test DTO object
            TestDTO testDataObject = new TestDTO
            {
                field1 = "field1",
                field2 = 10,
                field3 = true
            };

            var dto2 = new TestDTO2();

            var res = mapper.Map(testDataObject, dto2);
        }
    }
}
