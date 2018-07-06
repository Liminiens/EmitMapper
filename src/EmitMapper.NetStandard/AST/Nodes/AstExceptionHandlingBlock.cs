using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstExceptionHandlingBlock : IAstNode
    {
        readonly IAstNode _protectedBlock;
        readonly IAstNode _handlerBlock;
        readonly Type _exceptionType;
        readonly LocalBuilder _eceptionVariable;

        public AstExceptionHandlingBlock(
            IAstNode protectedBlock, 
            IAstNode handlerBlock, 
            Type exceptionType,
            LocalBuilder eceptionVariable)
        {
            this._protectedBlock = protectedBlock;
            this._handlerBlock = handlerBlock;
            this._exceptionType = exceptionType;
            this._eceptionVariable = eceptionVariable;
        }

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            var endBlock = context.ILGenerator.BeginExceptionBlock();
            _protectedBlock.Compile(context);
            context.ILGenerator.BeginCatchBlock(_exceptionType);
            context.ILGenerator.Emit(OpCodes.Stloc, _eceptionVariable);
            _handlerBlock.Compile(context);
            context.ILGenerator.EndExceptionBlock();
        }

        #endregion
    }
}