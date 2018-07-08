using System;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadThis : IAstRefOrAddr
    {
        public Type ThisType;

        public Type ItemType
        {
            get
            {
                return ThisType;
            }
        }

        public AstReadThis()
        {
        }

        public virtual void Compile(CompilationContext context)
        {
            AstReadArgument arg = new AstReadArgument()
            {
                ArgumentIndex = 0,
                ArgumentType = ThisType
            };
            arg.Compile(context);
        }
    }

    class AstReadThisRef : AstReadThis, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstReadThisAddr : AstReadThis, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }
}