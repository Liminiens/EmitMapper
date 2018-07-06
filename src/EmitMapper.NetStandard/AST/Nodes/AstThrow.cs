using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstThrow: IAstNode
    {
        public IAstRef exception;

        public void Compile(CompilationContext context)
        {
            exception.Compile(context);
            context.Emit(OpCodes.Throw);
        }
    }
}