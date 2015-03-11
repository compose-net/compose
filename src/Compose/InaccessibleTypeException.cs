using System;

namespace Compose
{
	public class InaccessibleTypeException : Exception
	{
		public InaccessibleTypeException(Type serviceType, Type inaccessibleType)
			: base($"") { }
	}
}
