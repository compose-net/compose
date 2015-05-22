﻿using System.Linq;
using System.Reflection;

namespace Compose
{
    internal static class TypeInfoExtensions
    {
		public static TypeInfo[] GetGenericArguments(this TypeInfo typeInfo)
		{
			return typeInfo.GenericTypeArguments.Select(x => x.GetTypeInfo()).ToArray();
		}

		public static TypeInfo[] GetInterfaces(this TypeInfo typeInfo)
		{
			return typeInfo.ImplementedInterfaces.Select(x => x.GetTypeInfo()).ToArray();
		}
    }
}