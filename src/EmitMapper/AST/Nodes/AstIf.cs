using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstIf : IAstNode
    {
        public IAstValue Condition;
        public AstComplexNode TrueBranch;
        public AstComplexNode FalseBranch;

        public void Compile(CompilationContext context)
        {
            Label elseLabel = context.ILGenerator.DefineLabel();
            Label endIfLabel = context.ILGenerator.DefineLabel();

            Condition.Compile(context);
            context.Emit(OpCodes.Brfalse, elseLabel);

            TrueBranch?.Compile(context);
            if (FalseBranch != null)
            {
                context.Emit(OpCodes.Br, endIfLabel);
            }

            context.ILGenerator.MarkLabel(elseLabel);
            FalseBranch?.Compile(context);
            context.ILGenerator.MarkLabel(endIfLabel);
        }
    }
}