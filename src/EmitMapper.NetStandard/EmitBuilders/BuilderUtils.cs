using System.Collections.Generic;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;
using EmitMapper.NetStandard.AST.Nodes;

namespace EmitMapper.NetStandard.EmitBuilders
{
    class BuilderUtils
    {
        /// <summary>
        /// Copies an argument to local variable
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="argIndex"></param>
        /// <returns></returns>
        public static IAstNode InitializeLocal(LocalBuilder loc, int argIndex)
        {
            return new AstComplexNode()
            {
                Nodes =
                    new List<IAstNode>()
                    {
                        new AstInitializeLocalVariable(loc),
                        new AstWriteLocal()
                        {
                            LocalIndex = loc.LocalIndex,
                            LocalType = loc.LocalType,
                            Value = AstBuildHelper.ReadArgumentRV(argIndex, typeof(object))
                        }
                    }
            };
        }
    }
}
