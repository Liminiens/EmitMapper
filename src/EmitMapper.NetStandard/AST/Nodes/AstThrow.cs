using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstThrow: IAstNode
    {
        public IAstRef Exception;

        public void Compile(CompilationContext context)
        {
            Exception.Compile(context);
            context.Emit(OpCodes.Throw);
        }
    }
}