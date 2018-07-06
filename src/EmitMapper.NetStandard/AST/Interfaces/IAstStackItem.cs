using System;

namespace EmitMapper.NetStandard.AST.Interfaces
{
    interface IAstStackItem: IAstNode
    {
        Type itemType { get; }
    }
}