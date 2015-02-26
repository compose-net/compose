namespace Compose
{
	public interface IFactory<out TService>
	{
		TService GetService();
	}
}
