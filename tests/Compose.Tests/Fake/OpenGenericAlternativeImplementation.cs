using System;

namespace Compose.Tests.Fake
{
	public class OpenGenericAlternativeImplementation<Service> : OpenGenericService<Service>
	{
		public Type ServiceType => typeof(OpenGenericAlternativeImplementation<>);
	}
}
