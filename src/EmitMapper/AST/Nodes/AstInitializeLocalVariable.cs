using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstInitializeLocalVariable : IAstNode
    {
        public readonly Type LocalType;
        public readonly int LocalIndex;

        public AstInitializeLocalVariable()
        {
        }

        public AstInitializeLocalVariable(LocalBuilder loc)
        {
            LocalType = loc.LocalType;
            LocalIndex = loc.LocalIndex;
        }

        public void Compile(CompilationContext context)
        {
            if (LocalType.IsValueType)
            {
                context.Emit(OpCodes.Ldloca, LocalIndex);
                context.Emit(OpCodes.Initobj, LocalType);
            }
        }
    }
}