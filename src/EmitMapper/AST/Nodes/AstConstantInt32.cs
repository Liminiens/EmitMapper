using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstConstantInt32 : IAstValue
    {
        public Int32 Value;

        public Type ItemType => typeof(Int32);

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldc_I4, Value);
        }
    }
}