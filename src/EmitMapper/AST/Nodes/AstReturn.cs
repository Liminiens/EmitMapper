using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstReturn : IAstNode, IAstAddr
    {
        public Type ReturnType;
        public IAstRefOrValue ReturnValue;
        public Type ItemType => ReturnType;
        public void Compile(CompilationContext context)
        {
            ReturnValue.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.ItemType);
            context.Emit(OpCodes.Ret);
        }
    }
}