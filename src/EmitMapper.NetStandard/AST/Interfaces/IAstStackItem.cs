using System;

namespace EmitMapper.NetStandard.AST.Interfaces
{
    interface IAstStackItem: IAstNode
    {
        Type ItemType { get; }
    }
}