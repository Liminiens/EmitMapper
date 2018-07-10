using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstNewObject : IAstRef
    {
        public Type ObjectType;
        public IAstStackItem[] ConstructorParams;

        public AstNewObject()
        {
        }

        public AstNewObject(Type objectType, IAstStackItem[] constructorParams)
        {
            ObjectType = objectType;
            ConstructorParams = constructorParams;
        }
        public Type ItemType => ObjectType;
        public void Compile(CompilationContext context)
        {
            if (ReflectionUtils.IsNullable(ObjectType))
            {
                IAstRefOrValue underlyingValue;
                var underlyingType = Nullable.GetUnderlyingType(ObjectType);
                if (ConstructorParams == null || ConstructorParams.Length == 0)
                {
                    LocalBuilder temp = context.ILGenerator.DeclareLocal(underlyingType);
                    new AstInitializeLocalVariable(temp).Compile(context);
                    underlyingValue = AstBuildHelper.ReadLocalRV(temp);
                }
                else
                {
                    underlyingValue = (IAstValue)ConstructorParams[0];
                }

                ConstructorInfo constructor = ObjectType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                    null,
                    new[] { underlyingType },
                    null);

                underlyingValue.Compile(context);
                context.EmitNewObject(constructor);
            }
            else
            {
                Type[] types;
                if (ConstructorParams == null || ConstructorParams.Length == 0)
                {
                    types = new Type[0];
                }
                else
                {
                    types = ConstructorParams.Select(c => c.ItemType).ToArray();
                    foreach (var p in ConstructorParams)
                    {
                        p.Compile(context);
                    }
                }

                ConstructorInfo ci = ObjectType.GetConstructor(types);
                if (ci != null)
                {
                    context.EmitNewObject(ci);
                }
                else if (ObjectType.IsValueType)
                {
                    LocalBuilder temp = context.ILGenerator.DeclareLocal(ObjectType);
                    new AstInitializeLocalVariable(temp).Compile(context);
                    AstBuildHelper.ReadLocalRV(temp).Compile(context);
                }
                else
                {
                    throw new Exception(
                        $"Constructor for types [{types.ToCSV(",")}] not found in {ObjectType.FullName}"
                    );
                }
            }
        }
    }
}