using System;

namespace Compose
{
	public interface ITransition<TService>
	{
		TService Service { get; }
		bool Change<TImplementation>(TImplementation implementation) where TImplementation : TService;
	}
}
