using System;

namespace Compose.Tests.Fake
{
	public class OpenGenericImplementation<Service> : OpenGenericService<Service>
	{
		public Type ServiceType => typeof(OpenGenericImplementation<>);
	}
}
