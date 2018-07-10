﻿using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstExprNot : IAstValue
    {
        private readonly IAstRefOrValue _value;

        public AstExprNot(IAstRefOrValue value)
        {
            _value = value;
        }

        public Type ItemType => typeof(Int32);

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldc_I4_0);
            _value.Compile(context);
            context.Emit(OpCodes.Ceq);
        }
    }
}
