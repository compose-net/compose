namespace Compose
{
	public interface IDynamicManager<TInterface, TOriginal> : IDynamicRegister<TInterface>, ITransitionManager<TInterface>
		where TInterface : class where TOriginal : TInterface
	{ }
}
