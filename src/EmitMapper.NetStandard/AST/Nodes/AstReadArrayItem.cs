﻿using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstReadArrayItem : IAstStackItem
    {
        public IAstRef array;
        public int index;

        public Type itemType
        {
            get
            {
                return array.itemType.GetElementType();
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, index);
            context.Emit(OpCodes.Ldelem, itemType);
        }
    }

    class AstReadArrayItemRef : AstReadArrayItem, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstReadArrayItemValue: AstReadArrayItem, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, index);
            context.Emit(OpCodes.Ldelema, itemType);
        }
    }
}