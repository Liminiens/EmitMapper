using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadField: IAstStackItem
    {
        public IAstRefOrAddr SourceObject;
        public FieldInfo FieldInfo;

        public Type ItemType
        {
            get
            {
                return FieldInfo.FieldType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            SourceObject.Compile(context);
            context.Emit(OpCodes.Ldfld, FieldInfo);
        }
    }

    class AstReadFieldRef : AstReadField, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstReadFieldValue : AstReadField, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    class AstReadFieldAddr : AstReadField, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            SourceObject.Compile(context);
            context.Emit(OpCodes.Ldflda, FieldInfo);
        }
    }
}