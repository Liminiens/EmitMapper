using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadLocal : IAstStackItem
    {
        public int LocalIndex;
        public Type LocalType;

        public Type ItemType
        {
            get
            {
                return LocalType;
            }
        }

        public AstReadLocal()
        {
        }

        public AstReadLocal(LocalBuilder loc)
        {
            LocalIndex = loc.LocalIndex;
            LocalType = loc.LocalType;
        }

        public virtual void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldloc, LocalIndex);
        }
    }

    class AstReadLocalRef : AstReadLocal, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstReadLocalValue : AstReadLocal, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    class AstReadLocalAddr : AstReadLocal, IAstAddr
    {
        public AstReadLocalAddr(LocalBuilder loc)
        {
            LocalIndex = loc.LocalIndex;
            LocalType = loc.LocalType.MakeByRefType();
        }

        override public void Compile(CompilationContext context)
        {
            //CompilationHelper.CheckIsValue(itemType);
            context.Emit(OpCodes.Ldloca, LocalIndex);
        }
    }
}