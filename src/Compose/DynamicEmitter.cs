using System.Reflection;

namespace Compose
{
	public interface DynamicEmitter
	{
        TypeInfo GetManagedDynamicProxy(TypeInfo serviceTypeInfo);
	}
}
