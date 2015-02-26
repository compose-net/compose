namespace Compose
{
	public interface IDirectTransition<TService>
	{
		TService Service { get; }
		bool Change<TImplementation>(TImplementation implementation) where TImplementation : TService;
	}
}
