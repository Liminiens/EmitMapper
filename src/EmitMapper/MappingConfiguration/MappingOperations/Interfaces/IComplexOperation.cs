using System.Collections.Generic;

namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces
{
    interface IComplexOperation: IMappingOperation
    {
        List<IMappingOperation> Operations { get; set; }
    }
}
