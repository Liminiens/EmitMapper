using System.Collections.Generic;

namespace EmitMapper.NetStandard.MappingConfiguration.MappingOperations.Interfaces
{
    interface IComplexOperation: IMappingOperation
    {
        List<IMappingOperation> Operations { get; set; }
    }
}
