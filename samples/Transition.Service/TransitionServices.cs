using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Transition.Service
{
	public static class TransitionServices
	{
		internal static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration)
		{
			var describe = new ServiceDescriber(configuration);

			yield return describe.Transient<IHost, StandardHost>();
		}
	}
}
