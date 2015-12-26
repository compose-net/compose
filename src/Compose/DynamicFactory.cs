using System;
using System.Reflection;

namespace Compose
{                                               
	public interface DynamicFactory
	{
        TypeInfo GetDynamicProxy(TypeInfo serviceTypeInfo);
	}
}
