using EmitMapper.AST.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstCallMethodVoid : IAstNode
    {
        public AstCallMethodVoid(
            MethodInfo methodInfo,
            IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
        {
            MethodInfo = methodInfo;
            InvocationObject = invocationObject;
            Arguments = arguments;
        }

        protected MethodInfo MethodInfo { get; }
        protected IAstRefOrAddr InvocationObject { get; }
        protected List<IAstStackItem> Arguments { get; }

        public void Compile(CompilationContext context)
        {
            new AstCallMethod(MethodInfo, InvocationObject, Arguments).Compile(context);

            if (MethodInfo.ReturnType != typeof(void))
            {
                context.Emit(OpCodes.Pop);
            }
        }
    }
}