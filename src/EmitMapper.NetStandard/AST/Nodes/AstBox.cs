using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstBox : IAstRef
    {
        public IAstRefOrValue Value;

        #region IAstReturnValueNode Members

        public Type ItemType
        {
            get 
            {
                return Value.ItemType;  
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            Value.Compile(context);

            if (Value.ItemType.IsValueType)
            {
                context.Emit(OpCodes.Box, ItemType);
            }
        }

        #endregion
    }
}