using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstExprEquals : IAstValue
    {
        readonly IAstValue _leftValue;
        readonly IAstValue _rightValue;

        public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
        {
            this._leftValue = leftValue;
            this._rightValue = rightValue;
        }

        #region IAstReturnValueNode Members

        public Type ItemType
        {
            get { return typeof(Int32); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            _leftValue.Compile(context);
            _rightValue.Compile(context);
            context.Emit(OpCodes.Ceq);
        }

        #endregion
    }
}