using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstExceptionHandlingBlock : IAstNode
    {
        private readonly IAstNode _protectedBlock;
        private readonly IAstNode _handlerBlock;
        private readonly Type _exceptionType;
        private readonly LocalBuilder _eceptionVariable;

        public AstExceptionHandlingBlock(
            IAstNode protectedBlock,
            IAstNode handlerBlock,
            Type exceptionType,
            LocalBuilder eceptionVariable)
        {
            _protectedBlock = protectedBlock;
            _handlerBlock = handlerBlock;
            _exceptionType = exceptionType;
            _eceptionVariable = eceptionVariable;
        }
        public void Compile(CompilationContext context)
        {
            var endBlock = context.ILGenerator.BeginExceptionBlock();
            _protectedBlock.Compile(context);
            context.ILGenerator.BeginCatchBlock(_exceptionType);
            context.ILGenerator.Emit(OpCodes.Stloc, _eceptionVariable);
            _handlerBlock.Compile(context);
            context.ILGenerator.EndExceptionBlock();
        }
    }
}