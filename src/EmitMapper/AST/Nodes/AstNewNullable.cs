using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
	class AstNewNullable: IAstValue
	{
		private Type _nullableType;
		public Type ItemType
		{
			get 
			{
				return _nullableType;
			}
		}
		public void Compile(CompilationContext context)
		{
			throw new NotImplementedException();
		}
	}
}
