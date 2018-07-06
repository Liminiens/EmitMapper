using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.NetStandard.AST;
using EmitMapper.NetStandard.AST.Helpers;
using EmitMapper.NetStandard.AST.Interfaces;
using EmitMapper.NetStandard.AST.Nodes;
using EmitMapper.NetStandard.Conversion;
using EmitMapper.NetStandard.Mappers;
using EmitMapper.NetStandard.MappingConfiguration;
using EmitMapper.NetStandard.MappingConfiguration.MappingOperations;
using EmitMapper.NetStandard.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.NetStandard.Utils;

namespace EmitMapper.NetStandard.EmitBuilders
{
    class MappingOperationsProcessor
    {
        public LocalBuilder LocFrom;
        public LocalBuilder LocTo;
        public LocalBuilder LocState;
        public LocalBuilder LocException;
        public CompilationContext CompilationContext;
        public IEnumerable<IMappingOperation> Operations = new List<IMappingOperation>();
        public List<object> StoredObjects = new List<object>();
        public IMappingConfigurator MappingConfigurator;
        public ObjectMapperManager ObjectsMapperManager;
        public IRootMappingOperation RootOperation;
        public StaticConvertersManager StaticConvertersManager;

        public MappingOperationsProcessor()
        {
        }

        public MappingOperationsProcessor(MappingOperationsProcessor prototype)
        {
            LocFrom = prototype.LocFrom;
            LocTo = prototype.LocTo;
            LocState = prototype.LocState;
            LocException = prototype.LocException;
            CompilationContext = prototype.CompilationContext;
            Operations = prototype.Operations;
            StoredObjects = prototype.StoredObjects;
            MappingConfigurator = prototype.MappingConfigurator;
            ObjectsMapperManager = prototype.ObjectsMapperManager;
            RootOperation = prototype.RootOperation;
            StaticConvertersManager = prototype.StaticConvertersManager;
        }

        public IAstNode ProcessOperations()
        {
            var result = new AstComplexNode();
            foreach (var operation in Operations)
            {
                IAstNode completeOperation = null;
                int operationId = AddObjectToStore(operation);

                if (operation is OperationsBlock)
                {
                    completeOperation =
                        new MappingOperationsProcessor(this)
                        {
                            Operations = (operation as OperationsBlock).Operations
                        }.ProcessOperations();
                }
                else if (operation is ReadWriteComplex)
                {
                    completeOperation = Process_ReadWriteComplex(operation as ReadWriteComplex, operationId);
                }
                else if (operation is DestSrcReadOperation)
                {
                    completeOperation = ProcessDestSrcReadOperation(operation as DestSrcReadOperation, operationId);
                }
                else if (operation is SrcReadOperation)
                {
                    completeOperation = ProcessSrcReadOperation(operation as SrcReadOperation, operationId);
                }
                else if (operation is DestWriteOperation)
                {
                    completeOperation = ProcessDestWriteOperation(operation as DestWriteOperation, operationId);
                }
                else if (operation is ReadWriteSimple)
                {
                    completeOperation = ProcessReadWriteSimple(operation as ReadWriteSimple, operationId);
                }

                if (completeOperation == null)
                {
                    continue;
                }
                if (LocException != null)
                {
                    var tryCatch = CreateExceptionHandlingBlock(operationId, completeOperation);
                    result.Nodes.Add(tryCatch);
                }
                else
                {
                    result.Nodes.Add(completeOperation);
                }
            }
            return result;
        }

        private IAstNode ProcessReadWriteSimple(ReadWriteSimple readWriteSimple, int operationId)
        {
            IAstRefOrValue sourceValue = ReadSrcMappingValue(readWriteSimple, operationId);

            IAstRefOrValue convertedValue;

            if (readWriteSimple.NullSubstitutor != null && (ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType) || !readWriteSimple.Source.MemberType.IsValueType))
            {
                convertedValue = new AstIfTernar(
                    ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType)
                        ? (IAstValue)new AstExprNot(AstBuildHelper.ReadPropertyRV(new AstValueToAddr((IAstValue)sourceValue), readWriteSimple.Source.MemberType.GetProperty("HasValue")))
                        : new AstExprIsNull(sourceValue),
                        GetNullValue(readWriteSimple.NullSubstitutor), // source is null
                        AstBuildHelper.CastClass(
                            ConvertMappingValue(
                                readWriteSimple,
                                operationId,
                                sourceValue
                            ),
                            readWriteSimple.Destination.MemberType
                        )
                );
            }
            else
            {
                convertedValue =
                    ConvertMappingValue(
                        readWriteSimple,
                        operationId,
                        sourceValue
                    );
            }

            return WriteMappingValue(readWriteSimple, operationId, convertedValue);
        }

        private IAstNode ProcessDestWriteOperation(DestWriteOperation destWriteOperation, int operationId)
        {
            LocalBuilder locValueToWrite = null;
            locValueToWrite = this.CompilationContext.ILGenerator.DeclareLocal(destWriteOperation.Getter.Method.ReturnType);

            var cmdValue = new AstWriteLocal(
                locValueToWrite,
                AstBuildHelper.CallMethod(
                    destWriteOperation.Getter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                            GetStoredObject(operationId, typeof(DestWriteOperation)),
                            typeof(DestWriteOperation).GetProperty("Getter")
                        ),
                        destWriteOperation.Getter.GetType()
                    ),
                    new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadLocalRV(LocFrom),
                        AstBuildHelper.ReadLocalRV(LocState)
                    }
                )
            );

            return
                new AstComplexNode
                {
                    Nodes = new List<IAstNode>
                    {
                        cmdValue,
                        new AstIf()
                        {
                            Condition = new AstExprEquals(
                                (IAstValue)AstBuildHelper.ReadMembersChain(
                                    AstBuildHelper.ReadLocalRA(locValueToWrite),
                                    new[] { (MemberInfo)locValueToWrite.LocalType.GetField("action") }
                                ),
                                new AstConstantInt32() { Value = 0 }
                            ),
                            TrueBranch = new AstComplexNode
                            {
                                Nodes = new List<IAstNode>
                                {
                                     AstBuildHelper.WriteMembersChain(
                                        destWriteOperation.Destination.MembersChain,
                                        AstBuildHelper.ReadLocalRA(LocTo),
                                        AstBuildHelper.ReadMembersChain(
                                            AstBuildHelper.ReadLocalRA(locValueToWrite),
                                            new[] { (MemberInfo)locValueToWrite.LocalType.GetField("value") }
                                        )
                                    )
                                }
                            }
                        }
                    }
                };
        }

        private IAstNode ProcessSrcReadOperation(SrcReadOperation srcReadOperation, int operationId)
        {
            var value = AstBuildHelper.ReadMembersChain(
                AstBuildHelper.ReadLocalRA(LocFrom),
                srcReadOperation.Source.MembersChain
            );

            return WriteMappingValue(srcReadOperation, operationId, value);
        }

        private IAstNode Process_ReadWriteComplex(ReadWriteComplex op, int operationId)
        {
            if (op.Converter != null)
            {
                return
                    AstBuildHelper.WriteMembersChain(
                        op.Destination.MembersChain,
                        AstBuildHelper.ReadLocalRA(LocTo),
                        AstBuildHelper.CallMethod(
                            op.Converter.GetType().GetMethod("Invoke"),
                            new AstCastclassRef(
                                (IAstRef)AstBuildHelper.ReadMemberRV(
                                     GetStoredObject(operationId, typeof(ReadWriteComplex)),
                                     typeof(ReadWriteComplex).GetProperty("Converter")
                                ),
                                op.Converter.GetType()
                            ),
                            new List<IAstStackItem>()
                            {
                                AstBuildHelper.ReadMembersChain(
                                    AstBuildHelper.ReadLocalRA(LocFrom),
                                    op.Source.MembersChain
                                ),
                                AstBuildHelper.ReadLocalRV(LocState),
                            }
                        )
                    );
            }

            var result = new AstComplexNode();
            LocalBuilder origTempSrc, origTempDst;
            LocalBuilder tempSrc = CompilationContext.ILGenerator.DeclareLocal(op.Source.MemberType);
            LocalBuilder tempDst = CompilationContext.ILGenerator.DeclareLocal(op.Destination.MemberType);
            origTempSrc = tempSrc;
            origTempDst = tempDst;

            result.Nodes.Add(
                new AstWriteLocal(tempSrc, AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), op.Source.MembersChain)
                )
            );
            result.Nodes.Add(
                new AstWriteLocal(tempDst, AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocTo), op.Destination.MembersChain))
            );

            var writeNullToDest =
                new List<IAstNode>
                {
                    AstBuildHelper.WriteMembersChain(
                        op.Destination.MembersChain,
                        AstBuildHelper.ReadLocalRA(LocTo),
                        GetNullValue(op.NullSubstitutor)
                    )
                };

            // Target construction
            var initDest = new List<IAstNode>();
            var custCtr = op.TargetConstructor;
            if (custCtr != null)
            {
                int custCtrIdx = AddObjectToStore(custCtr);
                initDest.Add(
                    new AstWriteLocal(
                        tempDst,
                        AstBuildHelper.CallMethod(
                            custCtr.GetType().GetMethod("Invoke"),
                            GetStoredObject(custCtrIdx, custCtr.GetType()),
                            null
                        )
                    )
                );
            }
            else
            {
                initDest.Add(
                    new AstWriteLocal(tempDst, new AstNewObject(op.Destination.MemberType, null))
                );
            }

            var copying = new List<IAstNode>();

            // if destination is nullable, create a temp target variable with underlying destination type
            if (ReflectionUtils.IsNullable(op.Source.MemberType))
            {
                tempSrc = CompilationContext.ILGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Source.MemberType));
                copying.Add(
                    new AstWriteLocal(
                        tempSrc,
                        AstBuildHelper.ReadPropertyRV(
                            AstBuildHelper.ReadLocalRA(origTempSrc),
                            op.Source.MemberType.GetProperty("Value")
                        )
                    )
                );
            }

            // If destination is null, initialize it.
            if (ReflectionUtils.IsNullable(op.Destination.MemberType) || !op.Destination.MemberType.IsValueType)
            {
                copying.Add(
                    new AstIf()
                    {
                        Condition = ReflectionUtils.IsNullable(op.Destination.MemberType)
                            ? (IAstValue)new AstExprNot((IAstValue)AstBuildHelper.ReadPropertyRV(AstBuildHelper.ReadLocalRA(origTempDst), op.Destination.MemberType.GetProperty("HasValue")))
                            : new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempDst)),
                        TrueBranch = new AstComplexNode() { Nodes = initDest }
                    }
                );
                if (ReflectionUtils.IsNullable(op.Destination.MemberType))
                {
                    tempDst = CompilationContext.ILGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Destination.MemberType));
                    copying.Add(
                        new AstWriteLocal(
                            tempDst,
                            AstBuildHelper.ReadPropertyRV(
                                AstBuildHelper.ReadLocalRA(origTempDst),
                                op.Destination.MemberType.GetProperty("Value")
                            )
                        )
                    );
                }
            }

            // Suboperations
            copying.Add(
                new AstComplexNode()
                {
                    Nodes = new List<IAstNode>
                    {
                        new MappingOperationsProcessor(this)
                        {
                            Operations = op.Operations,
                            LocTo = tempDst,
                            LocFrom = tempSrc,
                            RootOperation = MappingConfigurator.GetRootMappingOperation(op.Source.MemberType, op.Destination.MemberType)
                        }.ProcessOperations()
                    }
                }
            );

            IAstRefOrValue processedValue;
            if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            {
                processedValue =
                    new AstNewObject(
                        op.Destination.MemberType,
                        new[]
                        {
                            AstBuildHelper.ReadLocalRV(tempDst)
                        }
                    );
            }
            else
            {
                processedValue = AstBuildHelper.ReadLocalRV(origTempDst);
            }

            if (op.ValuesPostProcessor != null)
            {
                int postProcessorId = AddObjectToStore(op.ValuesPostProcessor);
                processedValue =
                    AstBuildHelper.CallMethod(
                        op.ValuesPostProcessor.GetType().GetMethod("Invoke"),
                        GetStoredObject(postProcessorId, op.ValuesPostProcessor.GetType()),
                        new List<IAstStackItem>
                        {
                            processedValue,
                            AstBuildHelper.ReadLocalRV(LocState)
                        }
                    );
            }

            copying.Add(
                AstBuildHelper.WriteMembersChain(
                    op.Destination.MembersChain,
                    AstBuildHelper.ReadLocalRA(LocTo),
                    processedValue
                )
            );

            if (ReflectionUtils.IsNullable(op.Source.MemberType) || !op.Source.MemberType.IsValueType)
            {
                result.Nodes.Add(
                    new AstIf()
                    {
                        Condition = ReflectionUtils.IsNullable(op.Source.MemberType)
                            ? (IAstValue)new AstExprNot((IAstValue)AstBuildHelper.ReadPropertyRV(AstBuildHelper.ReadLocalRA(origTempSrc), op.Source.MemberType.GetProperty("HasValue")))
                            : new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempSrc)),
                        TrueBranch = new AstComplexNode() { Nodes = writeNullToDest },
                        FalseBranch = new AstComplexNode() { Nodes = copying }
                    }
                );
            }
            else
            {
                result.Nodes.AddRange(copying);
            }

            return result;
        }

        private IAstRefOrValue GetNullValue(Delegate nullSubstitutor)
        {
            if (nullSubstitutor != null)
            {
                var substId = AddObjectToStore(nullSubstitutor);
                return
                    AstBuildHelper.CallMethod(
                        nullSubstitutor.GetType().GetMethod("Invoke"),
                        GetStoredObject(substId, nullSubstitutor.GetType()),
                        new List<IAstStackItem>
                        {
                            AstBuildHelper.ReadLocalRV(LocState)
                        }
                    );
            }
            else
            {
                return new AstConstantNull();
            }
        }
        private int AddObjectToStore(object obj)
        {
            int objectId = StoredObjects.Count();
            StoredObjects.Add(obj);
            return objectId;
        }

        private IAstNode CreateExceptionHandlingBlock(int mappingItemId, IAstNode writeValue)
        {
            var handler =
                new AstThrow
                {
                    Exception =
                        new AstNewObject
                        {
                            ObjectType = typeof(EmitMapperException),
                            ConstructorParams =
                                new IAstStackItem[]
                                {
                                    new AstConstantString()
                                    {
                                        Str = "Error in mapping operation execution: "
                                    },
                                    new AstReadLocalRef()
                                    {
                                        LocalIndex = LocException.LocalIndex,
                                        LocalType = LocException.LocalType
                                    },
                                    GetStoredObject(mappingItemId, typeof(IMappingOperation))
                                }
                        },
                };

            var tryCatch = new AstExceptionHandlingBlock(
                writeValue,
                handler,
                typeof(Exception),
                LocException
                );
            return tryCatch;
        }

        private IAstNode WriteMappingValue(
            IMappingOperation mappingOperation,
            int mappingItemId,
            IAstRefOrValue value)
        {
            IAstNode writeValue;

            if (mappingOperation is SrcReadOperation)
            {
                writeValue = AstBuildHelper.CallMethod(
                    typeof(ValueSetter).GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                             GetStoredObject(mappingItemId, typeof(SrcReadOperation)),
                             typeof(SrcReadOperation).GetProperty("Setter")
                         ),
                         (mappingOperation as SrcReadOperation).Setter.GetType()
                    ),
                    new List<IAstStackItem>()
                        {
                            AstBuildHelper.ReadLocalRV(LocTo),
                            value,
                            AstBuildHelper.ReadLocalRV(LocState),
                        }
                    );
            }
            else
            {
                writeValue = AstBuildHelper.WriteMembersChain(
                    (mappingOperation as IDestOperation).Destination.MembersChain,
                    AstBuildHelper.ReadLocalRA(LocTo),
                    value
                );
            }
            return writeValue;
        }

        private IAstRefOrValue ConvertMappingValue(
            ReadWriteSimple rwMapOp,
            int operationId,
            IAstRefOrValue sourceValue)
        {
            IAstRefOrValue convertedValue = sourceValue;
            if (rwMapOp.Converter != null)
            {
                convertedValue = AstBuildHelper.CallMethod(
                    rwMapOp.Converter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                             GetStoredObject(operationId, typeof(ReadWriteSimple)),
                             typeof(ReadWriteSimple).GetProperty("Converter")
                        ),
                        rwMapOp.Converter.GetType()
                    ),
                    new List<IAstStackItem>()
                    {
                        sourceValue,
                        AstBuildHelper.ReadLocalRV(LocState),
                    }
                );
            }
            else
            {
                if (rwMapOp.ShallowCopy && rwMapOp.Destination.MemberType == rwMapOp.Source.MemberType)
                {
                    convertedValue = sourceValue;
                }
                else
                {
                    var mi = StaticConvertersManager.GetStaticConverter(rwMapOp.Source.MemberType, rwMapOp.Destination.MemberType);
                    if (mi != null)
                    {
                        convertedValue = AstBuildHelper.CallMethod(
                            mi,
                            null,
                            new List<IAstStackItem> { sourceValue }
                        );
                    }
                    else
                    {
                        convertedValue = ConvertByMapper(rwMapOp);
                    }
                }
            }

            return convertedValue;
        }

        private IAstRefOrValue ConvertByMapper(ReadWriteSimple mapping)
        {
            IAstRefOrValue convertedValue;
            ObjectsMapperDescr mapper = ObjectsMapperManager.GetMapperInt(
                mapping.Source.MemberType,
                mapping.Destination.MemberType,
                MappingConfigurator);
            int mapperId = AddObjectToStore(mapper);

            convertedValue = AstBuildHelper.CallMethod(
                typeof(ObjectsMapperBaseImpl).GetMethod(
                    "Map",
                    new Type[] { typeof(object), typeof(object), typeof(object) }
                    ),

                new AstReadFieldRef
                {
                    FieldInfo = typeof(ObjectsMapperDescr).GetField("mapper"),
                    SourceObject = GetStoredObject(mapperId, mapper.GetType())
                },

                new List<IAstStackItem>()
                    {
                        AstBuildHelper.ReadMembersChain(
                            AstBuildHelper.ReadLocalRA(LocFrom),
                            mapping.Source.MembersChain
                        ),
                        AstBuildHelper.ReadMembersChain(
                            AstBuildHelper.ReadLocalRA(LocTo),
                            mapping.Destination.MembersChain
                        ),
                        (IAstRef)AstBuildHelper.ReadLocalRA(LocState)
                    }
                );
            return convertedValue;
        }

        private IAstNode ProcessDestSrcReadOperation(
            DestSrcReadOperation operation,
            int operationId)
        {
            IAstRefOrValue src =
                AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(LocFrom),
                    operation.Source.MembersChain
                );

            IAstRefOrValue dst =
                AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(LocFrom),
                    operation.Destination.MembersChain
                );

            return AstBuildHelper.CallMethod(
                typeof(ValueProcessor).GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(DestSrcReadOperation)),
                        typeof(DestWriteOperation).GetProperty("ValueProcessor")
                    ),
                    operation.ValueProcessor.GetType()
                ),
                new List<IAstStackItem> { src, dst, AstBuildHelper.ReadLocalRV(LocState) }
            );
        }

        private IAstRefOrValue ReadSrcMappingValue(
            IMappingOperation mapping,
            int operationId)
        {
            var readOp = mapping as ISrcReadOperation;
            if (readOp != null)
            {
                return AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(LocFrom),
                    readOp.Source.MembersChain
                );
            }

            var destWriteOp = (DestWriteOperation)mapping;
            if (destWriteOp.Getter != null)
            {
                return AstBuildHelper.CallMethod(
                    destWriteOp.Getter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                            GetStoredObject(operationId, typeof(DestWriteOperation)),
                            typeof(DestWriteOperation).GetProperty("Getter")
                        ),
                        destWriteOp.Getter.GetType()
                    ),
                    new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadLocalRV(LocState)
                    }
                );
            }
            throw new EmitMapperException("Invalid mapping operations");
        }

        private static IAstRef GetStoredObject(int objectIndex, Type castType)
        {
            var result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
                (IAstRef)AstBuildHelper.ReadFieldRA(
                    new AstReadThis() { ThisType = typeof(ObjectsMapperBaseImpl) },
                    typeof(ObjectsMapperBaseImpl).GetField(
                        "StroredObjects",
                        BindingFlags.Instance | BindingFlags.Public
                    )
                ),
                objectIndex
            );
            if (castType != null)
            {
                result = new AstCastclassRef(result, castType);
            }
            return result;
        }
    }
}
