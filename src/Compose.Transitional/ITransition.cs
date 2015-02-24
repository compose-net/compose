namespace Compose
{
	public interface ITransition<TService>
	{
		TService Service { get; }
		bool Change<TImplementation>() where TImplementation : TService;
	}
}
