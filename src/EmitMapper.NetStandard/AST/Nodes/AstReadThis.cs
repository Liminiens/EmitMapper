using System;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstReadThis : IAstRefOrAddr
    {
        public Type thisType;

        public Type itemType
        {
            get
            {
                return thisType;
            }
        }

        public AstReadThis()
        {
        }

        public virtual void Compile(CompilationContext context)
        {
            AstReadArgument arg = new AstReadArgument()
                                      {
                                          argumentIndex = 0,
                                          argumentType = thisType
                                      };
            arg.Compile(context);
        }
    }

    class AstReadThisRef : AstReadThis, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstReadThisAddr : AstReadThis, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }
}