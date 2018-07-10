using EmitMapper.Mappers;
using System;
using System.Reflection;
using System.Reflection.Emit;

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

        private static readonly AssemblyName AssemblyName;
        private static readonly AssemblyBuilder AssemblyBuilder;
        private static readonly ModuleBuilder ModuleBuilder;

        static DynamicAssemblyManager()
        {
            AssemblyName = new AssemblyName("EmitMapperAssembly");
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                AssemblyName,
                AssemblyBuilderAccess.RunAndCollect
                );

            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(AssemblyName.Name + ".dll");
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
                return ModuleBuilder.DefineType(
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
                return ModuleBuilder.DefineType(
                    CorrectTypeName(typeName),
                    TypeAttributes.Public,
                    parent,
                    null
                    );
            }
        }
    }
}