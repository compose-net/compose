using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Compose.Tests
{
	public class ServiceProviderTests
	{
		[Fact]
		public void CanHandleImplementationInstances()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddInstance(typeof(string), "foo"));
		}
	}
}
