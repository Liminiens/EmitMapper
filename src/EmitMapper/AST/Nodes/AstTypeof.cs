using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
	class AstTypeof: IAstRef
	{
		public Type Type;

		#region IAstStackItem Members

		public Type ItemType
		{
			get 
			{
				return typeof(Type);
			}
		}

		#endregion

		#region IAstNode Members

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ldtoken, Type);
			context.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
		}

		#endregion
	}
}
