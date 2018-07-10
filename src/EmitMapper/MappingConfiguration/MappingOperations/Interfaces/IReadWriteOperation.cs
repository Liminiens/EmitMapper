﻿using System;

namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces
{
    internal interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
    {
        Delegate NullSubstitutor { get; set; } // generic type: NullSubstitutor
        Delegate TargetConstructor { get; set; } // generic type: TargetConstructor
        Delegate Converter { get; set; }
    }
}
