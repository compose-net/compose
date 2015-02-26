using System;
using Microsoft.Framework.DependencyInjection;

namespace Compose
{
	public abstract class DefaultTransition<TService, TDefault> : ITransition<TService> where TDefault : TService
	{
		public TService Service { get; protected set; }

		public DefaultTransition(TService service) { Service = service; }

		public abstract bool Change<TImplementation>(TImplementation service) where TImplementation : TService;
	}
}
