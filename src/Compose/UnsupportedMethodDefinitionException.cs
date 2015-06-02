using System;
using System.Reflection;

namespace Compose
{
	public class UnsupportedMethodDefinitionException : Exception
	{
		public UnsupportedMethodDefinitionException(MethodInfo method, Exception inner)
			: base($"Unable to create dynamic method. {method.Name} has an unsupported definition.", inner) { }
	}
}
