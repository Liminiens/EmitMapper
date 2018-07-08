using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
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