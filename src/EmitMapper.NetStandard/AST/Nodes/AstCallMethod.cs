using System;
using System.Collections.Generic;
using System.Reflection;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstCallMethod: IAstRefOrValue
    {
        public MethodInfo MethodInfo;
        public IAstRefOrAddr InvocationObject;
        public List<IAstStackItem> Arguments;

		public AstCallMethod(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
		{
			if (methodInfo == null)
			{
				throw new InvalidOperationException("methodInfo is null");
			}
			this.MethodInfo = methodInfo;
			this.InvocationObject = invocationObject;
			this.Arguments = arguments;
		}

        public Type ItemType
        {
            get
            {
                return MethodInfo.ReturnType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            CompilationHelper.EmitCall(context, InvocationObject, MethodInfo, Arguments);
        }
    }

    class AstCallMethodRef : AstCallMethod, IAstRef
    {
        public AstCallMethodRef(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
            : base(methodInfo, invocationObject, arguments)
		{
		}

        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstCallMethodValue : AstCallMethod, IAstValue
    {
        public AstCallMethodValue(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
			: base(methodInfo, invocationObject, arguments)
		{
		}
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }
}