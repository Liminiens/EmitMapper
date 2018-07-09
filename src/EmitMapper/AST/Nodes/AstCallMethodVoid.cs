using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstCallMethodVoid : IAstNode
    {
        protected MethodInfo MethodInfo;
		protected IAstRefOrAddr InvocationObject;
        protected List<IAstStackItem> Arguments;

		public AstCallMethodVoid(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
		{
			this.MethodInfo = methodInfo;
			this.InvocationObject = invocationObject;
			this.Arguments = arguments;
		}

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