using System;
using System.Reflection;

namespace Compose
{
	public class UnsupportedPropertyDefinitionException : Exception
	{
		public UnsupportedPropertyDefinitionException(PropertyInfo property, Exception inner)
			: base($"Unable to create dynamic property. {property.Name} has an unsupported definition.", inner)
		{ }
	}
}
