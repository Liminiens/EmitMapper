using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstConstantNull : IAstRefOrValue
    {
        public Type ItemType => typeof(object);
        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldnull);
        }
    }
}