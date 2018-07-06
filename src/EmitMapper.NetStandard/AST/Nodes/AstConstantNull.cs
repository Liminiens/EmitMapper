using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstConstantNull : IAstRefOrValue
    {
        #region IAstReturnValueNode Members

        public Type ItemType
        {
            get { return typeof(object); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldnull);
        }

        #endregion
    }
}