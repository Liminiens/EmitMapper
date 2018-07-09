using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
	class AstExprNot : IAstValue
	{
	    readonly IAstRefOrValue _value;

		public Type ItemType
		{
			get { return typeof(Int32); }
		}

        public AstExprNot(IAstRefOrValue value)
		{
			_value = value;
		}

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ldc_I4_0);
			_value.Compile(context);
			context.Emit(OpCodes.Ceq);
		}
	}
}
