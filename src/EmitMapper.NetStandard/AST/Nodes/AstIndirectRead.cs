using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    abstract class AstIndirectRead : IAstStackItem
    {
        public Type ArgumentType;

        public Type ItemType
        {
            get
            {
                return ArgumentType;
            }
        }

        public abstract void Compile(CompilationContext context);
    }

    class AstIndirectReadRef : AstIndirectRead, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            context.Emit(OpCodes.Ldind_Ref, ItemType);
        }
    }

    class AstIndirectReadValue : AstIndirectRead, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            if (ItemType == typeof(Int32))
            {
                context.Emit(OpCodes.Ldind_I4);
            }
            else
            {
                throw new Exception("Unsupported type");
            }
        }
    }

    class AstIndirectReadAddr : AstIndirectRead, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
        }
    }
}