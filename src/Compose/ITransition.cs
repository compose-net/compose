namespace Compose
{
	public interface ITransition<TService>
	{
		bool Change(TService implementation);
	}
}
