using System;

namespace Compose
{
	internal static class TypeExtensions
	{
		internal static bool IsAssignableFromGeneric(this Type genericType, Type givenType)
		{
			if (genericType.IsInterface)
				foreach (var it in givenType.GetInterfaces())
					if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
						return true;

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return true;

			Type baseType = givenType.BaseType;
			if (baseType == null) return false;

			return genericType.IsAssignableFromGeneric(baseType);
		}
	}
}
