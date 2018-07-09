using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReturn : IAstNode, IAstAddr
    {
        public Type ReturnType;
        public IAstRefOrValue ReturnValue;

        public void Compile(CompilationContext context)
        {
            ReturnValue.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.ItemType);
            context.Emit(OpCodes.Ret);
        }

        public Type ItemType
        {
            get { return ReturnType; }
        }
    }
}