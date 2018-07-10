using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstReturnVoid : IAstNode
    {
        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ret);
        }
    }
}