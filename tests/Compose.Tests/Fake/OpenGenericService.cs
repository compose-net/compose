using System;

namespace Compose.Tests.Fake
{
	public interface OpenGenericService<Service>
	{
		Type ServiceType { get; }
	}
}
