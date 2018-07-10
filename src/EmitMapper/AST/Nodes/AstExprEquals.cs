using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstExprEquals : IAstValue
    {
        private readonly IAstValue _leftValue;
        private readonly IAstValue _rightValue;

        public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
        {
            _leftValue = leftValue;
            _rightValue = rightValue;
        }

        public Type ItemType => typeof(Int32);
        public void Compile(CompilationContext context)
        {
            _leftValue.Compile(context);
            _rightValue.Compile(context);
            context.Emit(OpCodes.Ceq);
        }
    }
}