﻿using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstInitializeLocalVariable: IAstNode
    {
        public Type LocalType;
        public int LocalIndex;

        public AstInitializeLocalVariable()
        {
        }

        public AstInitializeLocalVariable(LocalBuilder loc)
        {
            LocalType = loc.LocalType;
            LocalIndex = loc.LocalIndex;
        }

        public void Compile(CompilationContext context)
        {
            if(LocalType.IsValueType)
            {
                context.Emit(OpCodes.Ldloca, LocalIndex);
                context.Emit(OpCodes.Initobj, LocalType);
            }
        }
    }
}