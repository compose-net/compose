using System;
using System.Reflection;

namespace Compose
{
	internal static class TypeExtensions
	{
		internal static bool IsAssignableFromGeneric(this TypeInfo genericType, TypeInfo givenType)
		{
			if (genericType.IsInterface)
				foreach (var it in givenType.GetInterfaces())
					if (it.IsGenericType && it.GetGenericTypeDefinition().GetTypeInfo() == genericType)
						return true;

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition().GetTypeInfo() == genericType)
				return true;

			Type baseType = givenType.BaseType;
			if (baseType == null) return false;

			return genericType.IsAssignableFromGeneric(baseType.GetTypeInfo());
		}
	}
}
