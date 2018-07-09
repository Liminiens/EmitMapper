using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Mappers
{
    /// <summary>
    /// Mapper for collections. It can copy Array, List<>, ArrayList collections. 
    /// Collection type in source object and destination object can differ.
    /// </summary>
    public class MapperForCollectionImpl : CustomMapperImpl
    {
        private ObjectsMapperDescr _subMapper;

        /// <summary>
        /// Copies object properties and members of "fromEnumerable" to object "to"
        /// </summary>
        /// <param name="from">Source object</param>
        /// <param name="to">Destination object</param>
        /// <returns>Destination object</returns>
        public override object MapImpl(object from, object to, object state)
        {
            if (to == null && _targetConstructor != null)
            {
                to = _targetConstructor.CallFunc();
            }

            if (typeTo.IsArray)
            {
                if (from is IEnumerable)
                {
                    return CopyToArray((IEnumerable)from);
                }
                else
                {
                    return CopyScalarToArray(from);
                }
            }
            else if (typeTo.IsGenericType && typeTo.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (from is IEnumerable)
                {
                    return CopyToListInvoke((IEnumerable)from);
                }
                else
                {
                    return CopyToListScalarInvoke(from);
                }
            }
            else if (typeTo == typeof(ArrayList))
            {
                if (from is IEnumerable)
                {
                    return CopyToArrayList((IEnumerable)from);
                }
                else
                {
                    return CopyToArrayListScalar(from);
                }

            }
            else if (typeof(IList).IsAssignableFrom(typeTo))
            {
                return CopyToIList((IList)to, from);
            }
            return null;
        }

        private object CopyToIList(IList iList, object fromEnumerable)
        {
            if (iList == null)
            {
                iList = (IList)Activator.CreateInstance(typeTo);
            }
            foreach (object obj in fromEnumerable is IEnumerable enumerable ? enumerable : new[] { fromEnumerable })
            {
                if (obj == null)
                {
                    iList.Add(null);
                }
                if (_rootOperation == null || _rootOperation.ShallowCopy)
                {
                    iList.Add(obj);
                }
                else
                {
                    ObjectsMapperBaseImpl mapper = mapperMannager.GetMapperImpl(obj.GetType(), obj.GetType(), _mappingConfigurator);
                    iList.Add(mapper.Map(obj));
                }
            }
            return iList;
        }

        /// <summary>
        /// Copies object properties and members of "fromEnumerable" to object "to"
        /// </summary>
        /// <param name="from">Source object</param>
        /// <param name="to">Destination object</param>
        /// <returns>Destination object</returns>
        public override object Map(object from, object to, object state)
        {
            return base.Map(from, null, state);
        }

        /// <summary>
        /// Returns true if specified type is supported by this Mapper
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsSupportedType(Type type)
        {
            return
                type.IsArray ||
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) ||
                type == typeof(ArrayList) ||
                typeof(IList).IsAssignableFrom(type) ||
                typeof(IList<>).IsAssignableFrom(type)
                ;

        }

        /// <summary>
        /// Creates an instance of Mapper for collections.
        /// </summary>
        /// <param name="mapperName">Mapper name. It is used for registration in Mappers repositories.</param>
        /// <param name="mapperMannager">Mappers manager</param>
        /// <param name="typeFrom">Source type</param>
        /// <param name="typeTo">Destination type</param>
        /// <param name="subMapper"></param>
        /// <returns></returns>
        public static MapperForCollectionImpl CreateInstance(
            string mapperName,
            ObjectMapperManager mapperMannager,
            Type typeFrom,
            Type typeTo,
            ObjectsMapperDescr subMapper,
            IMappingConfigurator mappingConfigurator
            )
        {
            TypeBuilder tb = DynamicAssemblyManager.DefineType(
                "GenericListInv_" + mapperName,
                typeof(MapperForCollectionImpl)
                );

            if (typeTo.IsGenericType && typeTo.GetGenericTypeDefinition() == typeof(List<>))
            {
                MethodBuilder methodBuilder = tb.DefineMethod(
                    "CopyToListInvoke",
                    MethodAttributes.Family | MethodAttributes.Virtual,
                    typeof(object),
                    new[] { typeof(IEnumerable) }
                    );

                InvokeCopyImpl(typeTo, "CopyToList").Compile(new CompilationContext(methodBuilder.GetILGenerator()));

                methodBuilder = tb.DefineMethod(
                    "CopyToListScalarInvoke",
                    MethodAttributes.Family | MethodAttributes.Virtual,
                    typeof(object),
                    new[] { typeof(object) }
                    );

                InvokeCopyImpl(typeTo, "CopyToListScalar").Compile(
                    new CompilationContext(methodBuilder.GetILGenerator())
                    );
            }

            MapperForCollectionImpl result = (MapperForCollectionImpl)Activator.CreateInstance(tb.CreateTypeInfo().AsType());
            result.Initialize(mapperMannager, typeFrom, typeTo, mappingConfigurator, null);
            result._subMapper = subMapper;

            return result;
        }

        private static IAstNode InvokeCopyImpl(Type copiedObjectType, string copyMethodName)
        {
            var mi = typeof(MapperForCollectionImpl).GetMethod(
               copyMethodName,
               BindingFlags.Instance | BindingFlags.Public
            ).MakeGenericMethod(ExtractElementType(copiedObjectType));

            return new AstReturn()
            {
                ReturnType = typeof(object),
                ReturnValue = AstBuildHelper.CallMethod(
                               mi,
                               AstBuildHelper.ReadThis(typeof(MapperForCollectionImpl)),
                               new List<IAstStackItem>
                                   {
                                       new AstReadArgumentRef()
                                       {
                                           ArgumentIndex = 1,
                                           ArgumentType = typeof(object)
                                       }
                                   }
                               )
            };
        }

        private static Type ExtractElementType(Type collection)
        {
            if (collection.IsArray)
            {
                return collection.GetElementType();
            }
            if (collection == typeof(ArrayList))
            {
                return typeof(object);
            }
            if (collection.IsGenericType && collection.GetGenericTypeDefinition() == typeof(List<>))
            {
                return collection.GetGenericArguments()[0];
            }
            return null;

        }

        internal static Type GetSubMapperTypeTo(Type to)
        {
            return ExtractElementType(to);
        }

        internal static Type GetSubMapperTypeFrom(Type from)
        {
            Type result = ExtractElementType(from);
            if (result == null)
            {
                return from;
            }

            return result;
        }

        public override object CreateTargetInstance()
        {
            return null;
        }

        protected MapperForCollectionImpl() : base(null, null, null, null, null)
        {
        }

        private Array CopyToArray(IEnumerable from)
        {
            if (from is ICollection)
            {
                Array result = Array.CreateInstance(typeTo.GetElementType(), ((ICollection)from).Count);
                int idx = 0;
                foreach (object obj in from)
                {
                    result.SetValue(_subMapper.mapper.Map(obj), idx++);
                }
                return result;

            }
            else
            {
                ArrayList result = new ArrayList();
                foreach (object obj in from)
                {
                    result.Add(obj);
                }
                return result.ToArray(typeTo.GetElementType());
            }
        }

        private ArrayList CopyToArrayList(IEnumerable fromEnumerable)
        {
            if (ShallowCopy)
            {
                if (fromEnumerable is ICollection collection)
                {
                    return new ArrayList(collection);
                }

                ArrayList res = new ArrayList();
                foreach (object obj in fromEnumerable)
                {
                    res.Add(obj);
                }
                return res;
            }

            ArrayList result = new ArrayList();
            if (fromEnumerable is ICollection coll)
            {
                result = new ArrayList(coll.Count);
            }
            else
            {
                result = new ArrayList();
            }

            foreach (object obj in fromEnumerable)
            {
                if (obj == null)
                {
                    result.Add(null);
                }
                else
                {
                    ObjectsMapperBaseImpl mapper = mapperMannager.GetMapperImpl(obj.GetType(), obj.GetType(), _mappingConfigurator);
                    result.Add(mapper.Map(obj));
                }
            }
            return result;
        }

        private ArrayList CopyToArrayListScalar(object from)
        {
            ArrayList result = new ArrayList(1);
            if (ShallowCopy)
            {
                result.Add(from);
                return result;
            }
            ObjectsMapperBaseImpl mapper = mapperMannager.GetMapperImpl(from.GetType(), from.GetType(), _mappingConfigurator);
            result.Add(mapper.Map(from));
            return result;
        }

        protected List<T> CopyToList<T>(IEnumerable fromEnumerable)
        {
            List<T> result;
            if (fromEnumerable is ICollection collection)
            {
                result = new List<T>(collection.Count);
            }
            else
            {
                result = new List<T>();
            }
            foreach (object obj in fromEnumerable)
            {
                result.Add((T)_subMapper.mapper.Map(obj));
            }
            return result;
        }
        protected virtual object CopyToListInvoke(IEnumerable from)
        {
            return null;
        }

        protected List<T> CopyToListScalar<T>(object from)
        {
            List<T> result = new List<T>(1);
            result.Add((T)_subMapper.mapper.Map(from));
            return result;
        }
        protected virtual object CopyToListScalarInvoke(object from)
        {
            return null;
        }

        private Array CopyScalarToArray(object scalar)
        {
            Array result = Array.CreateInstance(typeTo.GetElementType(), 1);
            result.SetValue(_subMapper.mapper.Map(scalar), 0);
            return result;
        }
    }
}