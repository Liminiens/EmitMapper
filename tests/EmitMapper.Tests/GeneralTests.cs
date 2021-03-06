﻿using System;
using System.Reflection;
using EmitMapper.MappingConfiguration;
using Xunit;

namespace EmitMapper.Tests
{
    public class GeneralTests
    {
        public static void TestRefl(object obj)
        {
            FieldInfo field = obj.GetType().GetField("a");
            field.SetValue(obj, 10);
        }

        public class A
        {
            public enum En
            {
                En1,
                En2,
                En3
            }
            public class AInt
            {
                internal int intern = 13;
                public string str = "AInt";

                public AInt()
                {
                    intern = 13;
                }
            }

            private string m_str1 = "A::str1";

            public string str1
            {
                get
                {
                    return m_str1;
                }
                set
                {
                    m_str1 = value;
                }
            }

            public string str2 = "A::str2";
            public AInt obj;
            public En en = En.En3;

            int[] m_arr;

            public int[] arr
            {
                set
                {
                    m_arr = value;
                }
                get
                {
                    return m_arr;
                }
            }

            public AInt[] objArr;

            public string str3 = "A::str3";

            public A()
            {
                Console.WriteLine("A::A()");
            }
        }

        public class B
        {
            public enum En
            {
                En1,
                En2,
                En3
            }
            public class BInt
            {
                public string str = "BInt";
                /*
				public string str
				{
					get
					{
						throw new Exception("reading BInt::str");
					}
					set { }
				}
				 */
            }

            public string str1 = "B::str1";
            public string str2
            {
                get
                {
                    return "B::str2";
                }

            }
            public BInt obj = new BInt();
            public En en = En.En2;

            public BInt[] objArr;

            public int[] arr
            {
                get
                {
                    return new int[] { 1, 5, 9 };
                }
            }

            public object str3 = null;


            public B()
            {
                Console.WriteLine("B::B()");

                objArr = new BInt[2];
                objArr[0] = new BInt();
                objArr[0].str = "b objArr 1";
                objArr[1] = new BInt();
                objArr[1].str = "b objArr 2";
            }
        }

        internal class A1
        {
            public string f1 = "A1::f1";
            public string f2 = "A1::f2";
        }

        internal class B1
        {
            public string f1 = "B1::f1";
            public string f2 = "B1::f2";
        }

        public class Simple1
        {
            public int I = 10;
            public A.En fld1 = A.En.En1;
        }

        public class Simple2
        {
            public int I = 20;
            public B.En fld1 = B.En.En2;
        }

        [Fact]
        public void SimpleTest()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Simple2, Simple1>();
            //DynamicAssemblyManager.SaveAssembly();
            Simple1 s = mapper.Map(new Simple2());
            Assert.Equal(20, s.I);
            Assert.Equal(A.En.En2, s.fld1);
        }

        [Fact]
        public void SimpleTestEnum()
        {
            var mapper = Context.objMan.GetMapper<B.En, A.En>();
            //DynamicAssemblyManager.SaveAssembly();
            A.En aen = mapper.Map(B.En.En3);
            Assert.Equal(A.En.En3, aen);
        }

        public struct Struct1
        {
            public int fld;
        }

        public struct Struct2
        {
            public int fld;
        }

        [Fact]
        public void SimpleTestStruct()
        {
            var mapper = Context.objMan.GetMapper<Struct2, Struct1>();
            //DynamicAssemblyManager.SaveAssembly();
            Struct1 s = mapper.Map(new Struct2() { fld = 13 });
            Assert.Equal(13, s.fld);
        }

        public struct Class1
        {
            public int fld;
        }

        public struct Class2
        {
            public int fld;
        }

        [Fact]
        public void SimpleTestClass()
        {
            var mapper = Context.objMan.GetMapper<Class2, Class1>();
            //DynamicAssemblyManager.SaveAssembly();
            Class1 s = mapper.Map(new Class2() { fld = 13 });
            Assert.Equal(13, s.fld);
        }


        [Fact]
        public void GeneralTests_Test1()
        {
            A a = new A();
            B b = new B();
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(new DefaultMapConfig().DeepMap());
            //DynamicAssemblyManager.SaveAssembly();
            mapper.Map(b, a);
            Assert.Equal(A.En.En2, a.en);
            Assert.Equal(a.str1, b.str1);
            Assert.Equal(a.str2, b.str2);
            Assert.Equal(a.obj.str, b.obj.str);
            Assert.Equal(13, a.obj.intern);
            Assert.Equal(a.arr.Length, b.arr.Length);
            Assert.Equal(a.arr[0], b.arr[0]);
            Assert.Equal(a.arr[1], b.arr[1]);
            Assert.Equal(a.arr[2], b.arr[2]);

            Assert.Equal(a.objArr.Length, b.objArr.Length);
            Assert.Equal(a.objArr[0].str, b.objArr[0].str);
            Assert.Equal(a.objArr[1].str, b.objArr[1].str);
            Assert.True(a.str3 == null);
        }

        [Fact]
        public void GeneralTests_Test2()
        {
            A a = new A();
            B b = new B();

            a.obj = new A.AInt();
            b.obj = null;

            var mapper = Context.objMan.GetMapper<B, A>();
            //DynamicAssemblyManager.SaveAssembly();
            mapper.Map(b, a);
            Assert.True(a.obj == null);
        }

        [Fact]
        public void GeneralTests_Test3()
        {
            A a = new A();
            B b = new B();
            a.obj = new A.AInt();
            a.obj.intern = 15;

            var mapper = Context.objMan.GetMapper<B, A>();
            mapper.Map(b, a);
            Assert.Equal(15, a.obj.intern);
        }

        public class Source
        {
            public string field1 = "Source::field1";
            public string field2 = "Source::field2";
            public string field3 = "Source::field3";
        }

        public class Destination
        {
            public string m_field1;
            public string m_field2;
            public string m_field3;
        }

        [Fact]
        public void GeneralTests_Example2()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<Source, Destination>(
                new DefaultMapConfig().MatchMembers((m1, m2) => "m_" + m1 == m2)
            );

            Source src = new Source();
            Destination dst = mapper.Map(src);
            Assert.Equal(src.field1, dst.m_field1);
            Assert.Equal(src.field2, dst.m_field2);
            Assert.Equal(src.field3, dst.m_field3);
        }

        public class A2
        {
            public string str;
        }

        public class B2
        {
            public string str = "str";
        }

        [Fact]
        public void GeneralTests_ConvertUsing()
        {
            var a = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>(
                new DefaultMapConfig().ConvertUsing<string, string>(s => "converted " + s)
            ).Map(new B2());
            Assert.Equal("converted str", a.str);
        }

        [Fact]
        public void GeneralTests_Ignore()
        {
            var a = ObjectMapperManager.DefaultInstance.GetMapper<B, A>(
                new DefaultMapConfig().IgnoreMembers<B, A>(new[] { "str1" })
            ).Map(new B());
            Assert.Equal("A::str1", a.str1);
            Assert.Equal(A.En.En2, a.en);
        }

        public class A3
        {
            public class Int1
            {
                public string str1;
                public string str2;
                public int i;
            }

            public class Int2
            {
                public Int1 i1;
                public Int1 i2;
                public Int1 i3;
            }

            public Int2 i1;
            public Int2 i2;
            public Int2 i3;
        }

        public class B3
        {
            public class Int1
            {
                public string str1 = "1";
                public string str2 = null;
                public long i = 10;
            }

            public class Int2
            {
                public Int1 i1 = new Int1();
                public Int1 i2 = new Int1();
                public Int1 i3 = null;
            }

            public Int2 i1 = null;
            public Int2 i2 = new Int2();
            public Int2 i3 = new Int2();

        }

        [Fact]
        public void GeneralTests_Exception()
        {
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<B3, A3>();
            //DynamicAssemblyManager.SaveAssembly();
            var a = mapper.Map(new B3());
            Assert.True(a != null);
            Assert.True(a.i1 == null);
            Assert.True(a.i2 != null);
            Assert.True(a.i3 != null);
            Assert.True(a.i2.i1 != null);
            Assert.True(a.i2.i2 != null);
            Assert.True(a.i2.i3 == null);
            Assert.True(a.i3.i1 != null);
            Assert.True(a.i3.i2 != null);
            Assert.True(a.i3.i3 == null);

            Assert.Equal("1", a.i2.i1.str1);
            Assert.Equal("1", a.i2.i2.str1);
            Assert.Equal("1", a.i3.i1.str1);
            Assert.Equal("1", a.i3.i2.str1);
            Assert.True(a.i2.i1.str2 == null);
            Assert.True(a.i2.i2.str2 == null);
            Assert.True(a.i3.i1.str2 == null);
            Assert.True(a.i3.i2.str2 == null);
            Assert.Equal(10, a.i2.i1.i);
            Assert.Equal(10, a.i2.i2.i);
            Assert.Equal(10, a.i3.i1.i);
            Assert.Equal(10, a.i3.i2.i);
        }

        public class ConstructBy_Source
        {
            public class NestedClass
            {
                public string str = "ConstructBy_Source::str";
            }
            public NestedClass field = new NestedClass();
        }

        public class ConstructBy_Destination
        {
            public class NestedClass
            {
                public string str;
                public int i;
                public NestedClass(int i)
                {
                    str = "ConstructBy_Destination::str";
                    this.i = i;
                }
            }
            public NestedClass field;

            public ConstructBy_Destination(int i)
            {
            }
        }

        [Fact]
        public void ConstructByTest()
        {
            var mapper = ObjectMapperManager
                .DefaultInstance
                .GetMapper<ConstructBy_Source, ConstructBy_Destination>(
                    new DefaultMapConfig()
                        .ConstructBy<ConstructBy_Destination.NestedClass>(
                            () => new ConstructBy_Destination.NestedClass(3)
                        )
                        .ConstructBy<ConstructBy_Destination>(() => new ConstructBy_Destination(-1))
                );
            var d = mapper.Map(new ConstructBy_Source());
            Assert.Equal("ConstructBy_Source::str", d.field.str);
            Assert.Equal(3, d.field.i);
        }

        [Fact]
        public void ConstructByTest2()
        {
            var mapper = ObjectMapperManager
                .DefaultInstance
                .GetMapper<string, Guid>(
                    new DefaultMapConfig()
                        .ConvertUsing<string, Guid>(s => new Guid(s))
                );
            var guid = Guid.NewGuid();
            var d = mapper.Map(guid.ToString());
            Assert.Equal(guid, d);
        }

        public class TreeNode
        {
            public string data;
            public TreeNode next;
            public TreeNode[] subNodes;
        }

        [Fact]
        public void TestRecursiveClass()
        {
            var tree = new TreeNode
            {
                data = "node 1",
                next = new TreeNode
                {
                    data = "node 2",
                    next = new TreeNode
                    {
                        data = "node 3",
                        subNodes = new[]
                        {
                            new TreeNode
                            {
                                data = "sub sub data 1"
                            },
                            new TreeNode
                            {
                                data = "sub sub data 2"
                            }
                        }

                    }
                },
                subNodes = new[]
                {
                    new TreeNode
                    {
                        data = "sub data 1"
                    }
                }
            };
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TreeNode, TreeNode>(
                new DefaultMapConfig().DeepMap()
            );
            var tree2 = mapper.Map(tree);
            Assert.Equal("node 1", tree2.data);
            Assert.Equal("node 2", tree2.next.data);
            Assert.Equal("node 3", tree2.next.next.data);
            Assert.Equal("sub data 1", tree2.subNodes[0].data);
            Assert.Equal("sub sub data 1", tree2.next.next.subNodes[0].data);
            Assert.Equal("sub sub data 2", tree2.next.next.subNodes[1].data);
            Assert.True(tree2.next.next.next == null);
        }
    }
}