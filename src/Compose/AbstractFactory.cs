namespace Compose
{
	public interface AbstractFactory<out Service>
	{
		Service Create();
	}
}
