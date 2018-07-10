using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstExprIsNull : IAstValue
    {
        private readonly IAstRefOrValue _value;

        public AstExprIsNull(IAstRefOrValue value)
        {
            this._value = value;
        }
        public Type ItemType
        {
            get { return typeof(Int32); }
        }
        public void Compile(CompilationContext context)
        {
            if (!(_value is IAstRef) && !ReflectionUtils.IsNullable(_value.ItemType))
            {
                context.Emit(OpCodes.Ldc_I4_1);
            }
            else if (ReflectionUtils.IsNullable(_value.ItemType))
            {
                AstBuildHelper.ReadPropertyRV(
                    new AstValueToAddr((IAstValue)_value),
                    _value.ItemType.GetProperty("HasValue")
                ).Compile(context);
                context.Emit(OpCodes.Ldc_I4_0);
                context.Emit(OpCodes.Ceq);
            }
            else
            {
                _value.Compile(context);
                new AstConstantNull().Compile(context);
                context.Emit(OpCodes.Ceq);
            }
        }
    }
}
