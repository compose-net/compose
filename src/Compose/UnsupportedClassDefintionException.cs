using System;

namespace Compose
{
	public class UnsupportedClassDefintionException : Exception
	{
		public UnsupportedClassDefintionException(Type definition, Exception inner)
			: base($"Unable to create dynamic proxy. {definition.FullName} contains an unsupported defintion.", inner) { }
	}
}
