using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstWriteLocal : IAstNode
    {
        public int LocalIndex;
        public Type LocalType;
        public IAstRefOrValue Value;

        public AstWriteLocal()
        {
        }

        public AstWriteLocal(LocalBuilder loc, IAstRefOrValue value)
        {
            LocalIndex = loc.LocalIndex;
            LocalType = loc.LocalType;
            this.Value = value;
        }


        public void Compile(CompilationContext context)
        {
            Value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, LocalType, Value.ItemType);
            context.Emit(OpCodes.Stloc, LocalIndex);
        }
    }
}