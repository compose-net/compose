using System;
using System.Reflection;

namespace Compose
{
	public interface DynamicEmitter
	{
		Type GetManagedDynamicProxy(TypeInfo serviceTypeInfo);
	}
}
