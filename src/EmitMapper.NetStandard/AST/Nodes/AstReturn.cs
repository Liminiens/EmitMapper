using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
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