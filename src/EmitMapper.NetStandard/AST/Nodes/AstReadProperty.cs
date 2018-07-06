using System;
using System.Reflection;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;

namespace EmitMapper.NetStandard.AST.Nodes
{
    class AstReadProperty : IAstRefOrValue
    {
        public IAstRefOrAddr SourceObject;
        public PropertyInfo PropertyInfo;

        public Type ItemType
        {
            get
            {
                return PropertyInfo.PropertyType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            MethodInfo mi = PropertyInfo.GetGetMethod();

            if (mi == null)
            {
                throw new Exception("Property " + PropertyInfo.Name + " doesn't have get accessor");
            }

            AstBuildHelper.CallMethod(mi, SourceObject, null).Compile(context);
        }
    }

    class AstReadPropertyRef : AstReadProperty, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    class AstReadPropertyValue : AstReadProperty, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }
}