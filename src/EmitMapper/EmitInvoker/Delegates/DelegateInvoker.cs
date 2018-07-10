﻿using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.EmitInvoker.Delegates
{
    public static class DelegateInvoker
    {
        private static readonly ThreadSafeCache<Type> TypesCache = new ThreadSafeCache<Type>();

        public static DelegateInvokerBase GetDelegateInvoker(Delegate del)
        {
            var typeName = "EmitMapper.DelegateCaller_" + del.ToString();

            Type callerType = TypesCache.Get(
                typeName,
                () =>
                {
                    if (del.Method.ReturnType == typeof(void))
                    {
                        return BuildActionCallerType(typeName, del);
                    }
                    else
                    {
                        return BuildFuncCallerType(typeName, del);
                    }
                }
            );

            DelegateInvokerBase result = (DelegateInvokerBase)Activator.CreateInstance(callerType);
            result._del = del;
            return result;
        }


        private static Type BuildFuncCallerType(string typeName, Delegate del)
        {
            var par = del.Method.GetParameters();
            Type funcCallerType;
            switch (par.Length)
            {
                case 0:
                    funcCallerType = typeof(DelegateInvokerAction_0);
                    break;
                case 1:
                    funcCallerType = typeof(DelegateInvokerAction_1);
                    break;
                case 2:
                    funcCallerType = typeof(DelegateInvokerAction_2);
                    break;
                case 3:
                    funcCallerType = typeof(DelegateInvokerAction_3);
                    break;
                default:
                    throw new EmitMapperException("too many method parameters");
            }

            var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

            MethodBuilder methodBuilder = tb.DefineMethod(
                "CallFunc",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object),
                Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray()
            );

            new AstReturn
            {
                ReturnType = typeof(object),
                ReturnValue = CreateCallDelegate(del, par)
            }.Compile(new CompilationContext(methodBuilder.GetILGenerator()));

            return tb.CreateTypeInfo().AsType();
        }

        private static Type BuildActionCallerType(string typeName, Delegate del)
        {
            var par = del.Method.GetParameters();

            Type actionCallerType;
            switch (par.Length)
            {
                case 0:
                    actionCallerType = typeof(DelegateInvokerAction_0);
                    break;
                case 1:
                    actionCallerType = typeof(DelegateInvokerAction_1);
                    break;
                case 2:
                    actionCallerType = typeof(DelegateInvokerAction_2);
                    break;
                case 3:
                    actionCallerType = typeof(DelegateInvokerAction_3);
                    break;
                default:
                    throw new EmitMapperException("too many method parameters");
            }

            var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

            MethodBuilder methodBuilder = tb.DefineMethod(
                "CallAction",
                MethodAttributes.Public | MethodAttributes.Virtual,
                null,
                Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray()
            );

            new AstComplexNode
            {
                Nodes = new List<IAstNode>
                {
                    CreateCallDelegate(del, par),
                    new AstReturnVoid()
                }
            }.Compile(new CompilationContext(methodBuilder.GetILGenerator()));

            return tb.CreateTypeInfo().AsType();
        }

        private static IAstRefOrValue CreateCallDelegate(Delegate del, ParameterInfo[] parameters)
        {
            return
                AstBuildHelper.CallMethod(
                    del.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        AstBuildHelper.ReadFieldRV(
                            new AstReadThis() { ThisType = typeof(DelegateInvokerBase) },
                            typeof(DelegateInvokerBase).GetField("_del", BindingFlags.Public | BindingFlags.Instance)
                        ),
                        del.GetType()
                    ),
                    parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, typeof(object))).ToList()
                );
        }
    }
}
