using System;

namespace Compose
{
	public class UnsupportedTypeDefintionException : Exception
	{
		public UnsupportedTypeDefintionException(Type definition, Exception inner)
			: base($"Unable to create dynamic proxy. {definition.FullName} contains an unsupported defintion.", inner) { }
	}
}
