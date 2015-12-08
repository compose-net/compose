namespace Compose
{
	public interface DynamicManager<Interface, OriginalService> : DynamicRegister<Interface>, TransitionManager<Interface>
		where Interface : class where OriginalService : Interface
	{ }
}
