namespace Compose
{
	public interface IFactoryTransition<TService> : ITransition<TService>
	{
		TService GetService();
	}
}
