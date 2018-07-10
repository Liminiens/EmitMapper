using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstIfTernar : IAstRefOrValue
    {
        public readonly IAstRefOrValue Condition;
        public readonly IAstRefOrValue TrueBranch;
        public readonly IAstRefOrValue FalseBranch;

        public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
        {
            if (trueBranch.ItemType != falseBranch.ItemType)
            {
                throw new EmitMapperException("Types mismatch");
            }

            Condition = condition;
            TrueBranch = trueBranch;
            FalseBranch = falseBranch;
        }

        public Type ItemType => TrueBranch.ItemType;

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
