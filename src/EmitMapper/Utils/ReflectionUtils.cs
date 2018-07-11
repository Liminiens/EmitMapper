﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils
{
    public class ReflectionUtils
    {
        public class MatchedMember
        {
            public MemberInfo First { get; set; }
            public MemberInfo Second { get; set; }
            public MatchedMember(MemberInfo first, MemberInfo second)
            {
                First = first;
                Second = second;
            }
        }

        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static MemberInfo[] GetPublicFieldsAndProperties(Type type)
        {
            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
                .ToArray();
        }

        public static MatchedMember[] GetCommonMembers(Type first, Type second, Func<string, string, bool> matcher)
        {
            if (matcher == null)
            {
                matcher = (f, s) => f == s;
            }
            var firstMembers = GetPublicFieldsAndProperties(first);
            var secondMembers = GetPublicFieldsAndProperties(first);
            var result = new List<MatchedMember>();
            foreach (var f in firstMembers)
            {
                var s = secondMembers.FirstOrDefault(sm => matcher(f.Name, sm.Name));
                if (s != null)
                {
                    result.Add(new MatchedMember(f, s));
                }
            }
            return result.ToArray();
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            switch (mi)
            {
                case PropertyInfo propertyInfo:
                    return propertyInfo.PropertyType;
                case FieldInfo fieldInfo:
                    return fieldInfo.FieldType;
                case MethodInfo methodInfo:
                    return methodInfo.ReturnType;
                default:
                    return null;
            }
        }

        public static bool HasDefaultConstructor(Type type)
        {
            return type.GetConstructor(new Type[0]) != null;
        }
    }
}