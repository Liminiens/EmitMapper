using EmitMapper.AST.Interfaces;
using System;

namespace EmitMapper.AST.Nodes
{
    internal class AstNewNullable : IAstValue
    {
        private Type _nullableType;
        public Type ItemType => _nullableType;

        public void Compile(CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
