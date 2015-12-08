using System;

namespace Compose
{
	internal static class ExceptionHelpers
	{
		internal static void ReThrow<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, Func<Exception, Exception> ex)
		{
			try { action(arg1, arg2, arg3); } catch (Exception inner) { throw ex(inner); }
		}

		internal static void ReThrow<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<Exception, Exception> ex)
		{
			try { action(arg1, arg2, arg3, arg4); } catch (Exception inner) { throw ex(inner); }
		}
	}
}
