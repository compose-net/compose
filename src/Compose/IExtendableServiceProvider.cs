using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	internal interface IExtendableServiceProvider : IServiceProvider
	{
		void Extend(ServiceDescriptor service);
	}
}
