using System.Collections.Generic;
using EmitMapper.NetStandard.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.NetStandard.MappingConfiguration.MappingOperations
{
    public class OperationsBlock: IComplexOperation
    {
        public List<IMappingOperation> Operations { get; set; }
    }
}
