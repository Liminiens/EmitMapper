using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.EmitBuilders
{
    internal class MappingBuilder
    {
        private Type _from;
        private Type _to;
        private readonly TypeBuilder _typeBuilder;
        private readonly IMappingConfigurator _mappingConfigurator;
        private readonly ObjectMapperManager _objectsMapperManager;
        public readonly List<object> StoredObjects;

        public MappingBuilder(
            ObjectMapperManager objectsMapperManager,
            Type from,
            Type to,
            TypeBuilder typeBuilder,
            IMappingConfigurator mappingConfigurator
            )
        {
            _objectsMapperManager = objectsMapperManager;
            _from = from;
            _to = to;
            _typeBuilder = typeBuilder;
            StoredObjects = new List<object>();
            _mappingConfigurator = mappingConfigurator;
        }

        public void BuildCopyImplMethod()
        {
            if (ReflectionUtils.IsNullable(_from))
            {
                _from = Nullable.GetUnderlyingType(_from);
            }
            if (ReflectionUtils.IsNullable(_to))
            {
                _to = Nullable.GetUnderlyingType(_to);
            }

            MethodBuilder methodBuilder = _typeBuilder.DefineMethod(
                "MapImpl",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object),
                new Type[] { typeof(object), typeof(object), typeof(object) }
                );

            ILGenerator ilGen = methodBuilder.GetILGenerator();
            CompilationContext compilationContext = new CompilationContext(ilGen);

            AstComplexNode mapperAst = new AstComplexNode();
            var locFrom = ilGen.DeclareLocal(_from);
            var locTo = ilGen.DeclareLocal(_to);
            var locState = ilGen.DeclareLocal(typeof(object));
            LocalBuilder locException = null;

            mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locFrom, 1));
            mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locTo, 2));
            mapperAst.Nodes.Add(BuilderUtils.InitializeLocal(locState, 3));

#if DEBUG
			locException = compilationContext.ILGenerator.DeclareLocal(typeof(Exception));
#endif
            var mappingOperations = _mappingConfigurator.GetMappingOperations(_from, _to);
            StaticConvertersManager staticConverter = _mappingConfigurator.GetStaticConvertersManager();
            mapperAst.Nodes.Add(
                new MappingOperationsProcessor()
                {
                    LocException = locException,
                    LocFrom = locFrom,
                    LocState = locState,
                    LocTo = locTo,
                    ObjectsMapperManager = _objectsMapperManager,
                    CompilationContext = compilationContext,
                    StoredObjects = StoredObjects,
                    Operations = mappingOperations,
                    MappingConfigurator = _mappingConfigurator,
                    RootOperation = _mappingConfigurator.GetRootMappingOperation(_from, _to),
                    StaticConvertersManager = staticConverter ?? StaticConvertersManager.DefaultInstance
                }.ProcessOperations()
            );
            mapperAst.Nodes.Add(
                new AstReturn()
                {
                    ReturnType = typeof(object),
                    ReturnValue = AstBuildHelper.ReadLocalRV(locTo)
                }
            );

            mapperAst.Compile(compilationContext);
        }
    }
}
