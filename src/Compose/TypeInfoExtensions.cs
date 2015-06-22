using System.Linq;
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

		internal static bool IsAccessible(this TypeInfo typeInfo, bool acceptInternalsVisibleTo)
		{
			if (typeInfo.IsPublic || typeInfo.IsNestedPublic || typeInfo.IsVisible)
				return true;
			if (acceptInternalsVisibleTo && typeInfo.Assembly.HasInternalsVisibleToComposeProxies())
				return true;
			return false;
		}

		private static bool HasInternalsVisibleToComposeProxies(this Assembly assembly)
		{
			return assembly
				.CustomAttributes.OfType<System.Runtime.CompilerServices.InternalsVisibleToAttribute>()
				.Any(attribute => attribute.AssemblyName == DynamicEmitter.AssemblyName);
		}
    }
}
