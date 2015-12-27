using System;

namespace Compose
{
	public class UnsupportedTypeDefinitionException : Exception
	{
		public UnsupportedTypeDefinitionException(Type definition, Exception inner)
			: base($"Unable to create dynamic proxy. {definition.FullName} contains an unsupported definition.", inner)
		{ }
	}
}
