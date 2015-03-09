namespace Compose
{
	public interface ITransition
	{
		void Snapshot();

		void Restore();
	}

	public interface ITransition<in TService> : ITransition
	{
		bool Change(TService implementation);
	}
}
