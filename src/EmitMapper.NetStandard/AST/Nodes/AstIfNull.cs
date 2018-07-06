using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
	/// <summary>
	/// Generates "value ?? ifNullValue" expression.
	/// </summary>
	class AstIfNull : IAstRefOrValue
	{
	    readonly IAstRef _value;
	    readonly IAstRefOrValue _ifNullValue;

		public Type ItemType
		{
			get 
			{
				return _value.ItemType;
			}
		}

		public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
		{
			_value = value;
			_ifNullValue = ifNullValue;
			if (!_value.ItemType.IsAssignableFrom(_ifNullValue.ItemType))
			{
				throw new EmitMapperException("Incorrect ifnull expression");
			}
		}

		public void Compile(CompilationContext context)
		{
			Label ifNotNullLabel = context.ILGenerator.DefineLabel();
			_value.Compile(context);
			context.Emit(OpCodes.Dup);
			context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
			context.Emit(OpCodes.Pop);
			_ifNullValue.Compile(context);
			context.ILGenerator.MarkLabel(ifNotNullLabel);
		}
	}
}
