namespace Compose
{
	public interface IDirectTransition<TService> : ITransition<TService>
	{
		TService Service { get; }
	}
}
