using System;

namespace EmitMapper.NetStandard.MappingConfiguration
{
    public interface ICustomConverter
    {
        void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig);
    }
}
