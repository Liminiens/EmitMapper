using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;
using EmitMapper.NetStandard.AST.Nodes;
using EmitMapper.NetStandard.Utils;

namespace EmitMapper.NetStandard.EmitBuilders
{
    class CreateTargetInstanceBuilder
    {
		public static void BuildCreateTargetInstanceMethod(Type type, TypeBuilder typeBuilder)
		{
			if (ReflectionUtils.IsNullable(type))
			{
				type = Nullable.GetUnderlyingType(type);
			}

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(
				"CreateTargetInstance",
				MethodAttributes.Assembly | MethodAttributes.Virtual,
				typeof(object),
				null
				);

			ILGenerator ilGen = methodBuilder.GetILGenerator();
			CompilationContext context = new CompilationContext(ilGen);
			IAstRefOrValue returnValue;

			if (type.IsValueType)
			{
				LocalBuilder lb = ilGen.DeclareLocal(type);
				new AstInitializeLocalVariable(lb).Compile(context);

				returnValue =
					new AstBox()
					{
						value = AstBuildHelper.ReadLocalRV(lb)
					};
			}
			else
			{
				returnValue =
					ReflectionUtils.HasDefaultConstructor(type)
						?
							new AstNewObject()
							{
								objectType = type
							}
						:
							(IAstRefOrValue)new AstConstantNull();
			}
			new AstReturn()
			{
				returnType = type,
				returnValue = returnValue
			}.Compile(context);
		}
    }
}