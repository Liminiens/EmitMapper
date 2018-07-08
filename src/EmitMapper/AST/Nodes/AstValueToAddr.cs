using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstValueToAddr: IAstAddr
    {
        public IAstValue Value;
        public Type ItemType
        {
            get 
            {
                return Value.ItemType; 
            }
        }

        public AstValueToAddr(IAstValue value)
        {
            this.Value = value;
        }

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