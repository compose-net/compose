using Microsoft.Framework.DependencyInjection;

namespace Compose
{
	internal interface IObserveServiceCollectionChanges
	{
		void Next(ServiceDescriptor amendment);
	}
}
