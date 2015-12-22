using System;
using System.Reflection;

namespace Compose
{
	public interface DynamicFactory
	{
		Type GetDynamicProxy(TypeInfo serviceTypeInfo);
	}
}
