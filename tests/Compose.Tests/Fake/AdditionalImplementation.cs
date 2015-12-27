using System;

namespace Compose.Tests.Fake
{
	public class AdditionalImplementation : Service
	{
		public Type ServiceType { get; } = typeof(AdditionalImplementation);
	}
}
