using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadArgument : IAstStackItem
    {
        public int ArgumentIndex;
        public Type ArgumentType;

        public Type ItemType
        {
            get
            {
                return ArgumentType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            switch (ArgumentIndex)
            {
                case 0:
                    context.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    context.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    context.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    context.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    context.Emit(OpCodes.Ldarg, ArgumentIndex);
                    break;
            }
        }
    }

    class AstReadArgumentRef : AstReadArgument, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstReadArgumentValue : AstReadArgument, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    class AstReadArgumentAddr : AstReadArgument, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            context.Emit(OpCodes.Ldarga, ArgumentIndex);
        }
    }
}