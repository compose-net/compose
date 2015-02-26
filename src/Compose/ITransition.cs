namespace Compose
{
	public interface ITransition<TService>
	{
		bool Change<TImplementation>(TImplementation implementation) where TImplementation : TService;
	}
}
