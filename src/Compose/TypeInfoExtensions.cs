using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compose
{
	internal static class TypeInfoExtensions
	{
		public static TypeInfo[] GetGenericArguments(this TypeInfo typeInfo)
		{
			IEnumerable<Type> arguments = typeInfo.IsGenericTypeDefinition
								? typeInfo.GenericTypeParameters
								: typeInfo.GenericTypeArguments;
			return arguments.Select(x => x.GetTypeInfo()).ToArray();
		}

		internal static bool IsAccessible(this TypeInfo typeInfo, bool acceptInternalsVisibleTo)
		{
			if (typeInfo.IsPublic || typeInfo.IsNestedPublic || typeInfo.IsVisible)
				return true;
			if (acceptInternalsVisibleTo && typeInfo.Assembly.HasInternalsVisibleToComposeProxies())
				return true;
			return false;
		}

		private static bool HasInternalsVisibleToComposeProxies(this Assembly assembly)
			=> assembly
				.CustomAttributes.OfType<System.Runtime.CompilerServices.InternalsVisibleToAttribute>()
				.Any(attribute => attribute.AssemblyName == IlGeneratingDynamicEmitter.AssemblyName);
	}
}
