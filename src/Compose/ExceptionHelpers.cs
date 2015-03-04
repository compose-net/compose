using System;

namespace Compose
{
	internal static class ExceptionHelpers
	{
		internal static void ReThrow(this Action action, Func<Exception, Exception> ex)
		{
			try { action(); } catch (Exception inner) { throw ex(inner); }
		}

		internal static void ReThrow<T>(this Action<T> action, T arg, Func<Exception, Exception> ex)
		{
			try { action(arg); } catch (Exception inner) { throw ex(inner); }
		}

		internal static void ReThrow<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, Func<Exception, Exception> ex)
		{
			try { action(arg1, arg2); } catch (Exception inner) { throw ex(inner); }
		}

		internal static void ReThrow<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, Func<Exception, Exception> ex)
		{
			try { action(arg1, arg2, arg3); } catch (Exception inner) { throw ex(inner); }
		}

		internal static void ReThrow<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<Exception, Exception> ex)
		{
			try { action(arg1, arg2, arg3, arg4); } catch (Exception inner) { throw ex(inner); }
		}

		internal static TResult ReThrow<TResult>(this Func<TResult> action, Func<Exception, Exception> ex)
		{
			try { return action(); } catch (Exception inner) { throw ex(inner); }
		}

		internal static TResult ReThrow<T, TResult>(this Func<T, TResult> action, T arg, Func<Exception, Exception> ex)
		{
			try { return action(arg); } catch (Exception inner) { throw ex(inner); }
		}

		internal static TResult ReThrow<T1, T2, TResult>(this Func<T1, T2, TResult> action, T1 arg1, T2 arg2, Func<Exception, Exception> ex)
		{
			try { return action(arg1, arg2); } catch (Exception inner) { throw ex(inner); }
		}

		internal static TResult ReThrow<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> action, T1 arg1, T2 arg2, T3 arg3, Func<Exception, Exception> ex)
		{
			try { return action(arg1, arg2, arg3); } catch (Exception inner) { throw ex(inner); }
		}

		internal static TResult ReThrow<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<Exception, Exception> ex)
		{
			try { return action(arg1, arg2, arg3, arg4); } catch (Exception inner) { throw ex(inner); }
		}
	}
}
