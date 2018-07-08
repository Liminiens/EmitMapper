using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstConstantInt32 : IAstValue
    {
        public Int32 Value;

        #region IAstReturnValueNode Members

        public Type ItemType
        {
            get { return typeof(Int32); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldc_I4, Value);
        }

        #endregion
    }
}