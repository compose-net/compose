using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal class TransitionalServiceProvider : RootServiceProvider
	{
		private readonly Dictionary<Type, object> _transitions;

		internal TransitionalServiceProvider(IEnumerable<IServiceDescriptor> services)
			: base(new WrappedServiceProvider(services.WithSelfBoundTransitionals()))
		{
			_transitions = services.Where(x => x.IsTransition()).ToDictionary(x => x.ServiceType, x => base.GetService(x.ImplementationType));
		}

		internal TransitionalServiceProvider(IServiceProvider provider, TransitionalServiceProvider root) : base(provider)
		{
			_transitions = root._transitions;
		}

		internal TransitionalServiceProvider(IEnumerable<IServiceDescriptor> services, RootServiceProvider provider) 
			: base(provider, provider)
		{
			_transitions = services.Where(x => typeof(ITransition<>).IsAssignableFrom(x.ImplementationType) && x.Lifecycle == LifecycleKind.Singleton)
				.ToDictionary(x => x.ServiceType, x => provider.GetService(x.ImplementationType));
		}

		internal override RootServiceProvider Extend(IEnumerable<IServiceDescriptor> services)
		{
			return new TransitionalServiceProvider(new WrappedServiceProvider(services), this);
		}

		public override object GetService(Type serviceType)
		{
			if (_transitions.ContainsKey(serviceType)) return _transitions[serviceType];
			return base.GetService(serviceType);
		}
	}
}
