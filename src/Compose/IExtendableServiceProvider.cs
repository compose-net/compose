using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	internal interface IExtendableServiceProvider : IServiceProvider
	{
		void Snapshot();

		void Restore();

		void Subscribe(IObserveServiceCollectionChanges subscriber);

		IExtendableServiceProvider Extend(ServiceDescriptor service);
	}
}
