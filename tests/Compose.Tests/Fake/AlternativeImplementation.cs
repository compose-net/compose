using System;

namespace Compose.Tests.Fake
{
	public class AlternativeImplementation : Service
	{
		public Type ServiceType { get; } = typeof(AlternativeImplementation);
	}
}
