using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Transition.Service
{
	public static class TransitionServices
	{
		internal static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			yield return ServiceDescriptor.Transient<IHost, StandardHost>();
		}
	}
}
