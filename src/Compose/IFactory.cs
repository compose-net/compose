namespace Compose
{
	public interface IFactory<TService> : ITransition<TService>
	{
		TService GetService();
	}
}
