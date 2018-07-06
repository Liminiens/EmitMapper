using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstWriteField: IAstNode
    {
        public IAstRefOrAddr TargetObject;
        public IAstRefOrValue Value;
        public FieldInfo FieldInfo;

        public void Compile(CompilationContext context)
        {
            TargetObject.Compile(context);
            Value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, FieldInfo.FieldType, Value.ItemType);
            context.Emit(OpCodes.Stfld, FieldInfo);
        }
    }
}