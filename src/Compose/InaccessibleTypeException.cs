using System;
using System.Reflection;

namespace Compose
{
	public class InaccessibleTypeException : Exception
	{
		public InaccessibleTypeException(TypeInfo serviceType)
			: base($"{serviceType.FullName} must be public.")
		{ }

		public InaccessibleTypeException(TypeInfo serviceType, TypeInfo inaccessibleType)
			: base($"The {inaccessibleType.FullName} generic on {serviceType.FullName} must be public.")
		{ }
	}
}
