using System;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;
using EmitMapper.NetStandard.Utils;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstExprIsNull : IAstValue
    {
        IAstRefOrValue value;

		public AstExprIsNull(IAstRefOrValue value)
        {
            this.value = value;
        }

        #region IAstReturnValueNode Members

        public Type itemType
        {
            get { return typeof(Int32); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            if (!(value is IAstRef) && !ReflectionUtils.IsNullable(value.itemType))
            {
                context.Emit(OpCodes.Ldc_I4_1);
            }
			else if (ReflectionUtils.IsNullable(value.itemType))
			{
				AstBuildHelper.ReadPropertyRV(
					new AstValueToAddr((IAstValue)value),
					value.itemType.GetProperty("HasValue")
				).Compile(context);
				context.Emit(OpCodes.Ldc_I4_0);
				context.Emit(OpCodes.Ceq);
			}
			else
			{
				value.Compile(context);
				new AstConstantNull().Compile(context);
				context.Emit(OpCodes.Ceq);
			}
        }

        #endregion
    }
}
