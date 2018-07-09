using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
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