using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstIfTernar : IAstRefOrValue
    {
        public IAstRefOrValue Condition;
        public IAstRefOrValue TrueBranch;
        public IAstRefOrValue FalseBranch;

        #region IAstNode Members

        public Type ItemType
        {
            get 
            {
                return TrueBranch.ItemType;
            }
        }

        public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
        {
            if (trueBranch.ItemType != falseBranch.ItemType)
            {
                throw new EmitMapperException("Types mismatch");
            }

            this.Condition = condition;
            this.TrueBranch = trueBranch;
            this.FalseBranch = falseBranch;
        }

        public void Compile(CompilationContext context)
        {
            Label elseLabel = context.ILGenerator.DefineLabel();
            Label endIfLabel = context.ILGenerator.DefineLabel();

            Condition.Compile(context);
            context.Emit(OpCodes.Brfalse, elseLabel);

            if (TrueBranch != null)
            {
                TrueBranch.Compile(context);
            }
            if (FalseBranch != null)
            {
                context.Emit(OpCodes.Br, endIfLabel);
            }

            context.ILGenerator.MarkLabel(elseLabel);
            if (FalseBranch != null)
            {
                FalseBranch.Compile(context);
            }
            context.ILGenerator.MarkLabel(endIfLabel);
        }

        #endregion
    }
}
