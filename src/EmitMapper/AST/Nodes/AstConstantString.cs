using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstConstantString : IAstRef
    {
        public string Str;
        public Type ItemType => typeof(string);
        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldstr, Str);
        }
    }
}