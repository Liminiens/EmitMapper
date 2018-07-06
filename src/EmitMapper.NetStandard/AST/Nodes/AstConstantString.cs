using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstConstantString: IAstRef
    {
        public string Str;

        #region IAstStackItem Members

        public Type ItemType
        {
            get 
            {
                return typeof(string);
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldstr, Str);
        }

        #endregion
    }
}