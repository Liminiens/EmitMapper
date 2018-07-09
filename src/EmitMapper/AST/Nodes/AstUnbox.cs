using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstUnbox : IAstValue
    {
        public Type UnboxedType;
        public IAstRef RefObj;

        public Type ItemType
        {
            get { return UnboxedType; }
        }

        public void Compile(CompilationContext context)
        {
            RefObj.Compile(context);
            context.Emit(OpCodes.Unbox_Any, UnboxedType);
        }
    }
}