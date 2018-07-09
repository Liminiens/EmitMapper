﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.Mappers;

namespace EmitMapper
{
    /// <summary>
    /// Class which maintains an assembly for created object Mappers
    /// </summary>
    public class DynamicAssemblyManager
    {
        ///// <summary>
        ///// Saves assembly with created Mappers to file. This method is useful for debugging purpose.
        ///// </summary>
        //public static void SaveAssembly()
        //{
        //    lock (typeof(DynamicAssemblyManager))
        //    {
        //        assemblyBuilder.Save(assemblyName.Name + ".dll");
        //    }
        //}

        #region Non-public members

        private static AssemblyName assemblyName;
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;

        static DynamicAssemblyManager()
        {
            assemblyName = new AssemblyName("EmitMapperAssembly");
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndCollect
                );

            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");
        }

        private static string CorrectTypeName(string typeName)
        {
            if (typeName.Length >= 1042)
            {
                typeName = "type_" + typeName.Substring(0, 900) + Guid.NewGuid().ToString().Replace("-", "");
            }
            return typeName;
        }

        internal static TypeBuilder DefineMapperType(string typeName)
        {
            lock (typeof(DynamicAssemblyManager))
            {
                return moduleBuilder.DefineType(
                    CorrectTypeName(typeName + Guid.NewGuid().ToString().Replace("-", "")),
                    TypeAttributes.Public,
                    typeof(MapperForClassImpl),
                    null
                    );
            }
        }

        internal static TypeBuilder DefineType(string typeName, Type parent)
        {
            lock (typeof(DynamicAssemblyManager))
            {
                return moduleBuilder.DefineType(
                    CorrectTypeName(typeName),
                    TypeAttributes.Public,
                    parent,
                    null
                    );
            }
        }
        #endregion
    }
}