using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstReadArrayItem : IAstStackItem
    {
        public IAstRef Array;
        public int Index;

        public Type ItemType
        {
            get
            {
                return Array.ItemType.GetElementType();
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            Array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, Index);
            context.Emit(OpCodes.Ldelem, ItemType);
        }
    }

    class AstReadArrayItemRef : AstReadArrayItem, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstReadArrayItemValue: AstReadArrayItem, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            Array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, Index);
            context.Emit(OpCodes.Ldelema, ItemType);
        }
    }
}