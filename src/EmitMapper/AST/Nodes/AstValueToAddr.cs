using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstValueToAddr : IAstAddr
    {
        public readonly IAstValue Value;

        public AstValueToAddr(IAstValue value)
        {
            Value = value;
        }
        public Type ItemType => Value.ItemType;
        public void Compile(CompilationContext context)
        {
            LocalBuilder loc = context.ILGenerator.DeclareLocal(ItemType);
            new AstInitializeLocalVariable(loc).Compile(context);
            new AstWriteLocal()
            {
                LocalIndex = loc.LocalIndex,
                LocalType = loc.LocalType,
                Value = Value
            }.Compile(context);
            new AstReadLocalAddr(loc).Compile(context);
        }
    }
}