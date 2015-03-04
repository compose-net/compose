using System;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Compose
{
	internal abstract class BaseServiceProvider : IExtendableServiceProvider
	{
		private readonly List<IObserveServiceCollectionChanges> _subscribers = new List<IObserveServiceCollectionChanges>();

		public abstract IExtendableServiceProvider Extend(ServiceDescriptor service);

		public abstract object GetService(Type serviceType);

		protected void PublishChange(ServiceDescriptor amendment)
		{
			_subscribers.ForEach(x => x.Next(amendment));
		}

		public void Subscribe(IObserveServiceCollectionChanges subscriber)
		{
			_subscribers.Add(subscriber);
		}
	}
}
