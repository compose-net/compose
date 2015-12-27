using System;

namespace Compose.Tests.Fake
{
	public sealed class Implementation : Service
	{
		public Type ServiceType { get; } = typeof(Implementation);
	}
}
