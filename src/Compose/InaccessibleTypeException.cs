using System;

namespace Compose
{
	public class InaccessibleTypeException : Exception
	{
		public InaccessibleTypeException(Type serviceType)
			: base($"{serviceType.FullName} must be public.") { }

		public InaccessibleTypeException(Type serviceType, Type inaccessibleType)
			: base($"The {inaccessibleType.FullName} generic on {serviceType.FullName} must be public.") { }
	}
}
