using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Compose
{
    public sealed class Activate
    {
        private static readonly object[] _emptyObjectArray = new object[0];
        private static ConcurrentDictionary<TypeInfo, TypeCacheEntry> _typeCache = new ConcurrentDictionary<TypeInfo, TypeCacheEntry>();

        public static object Type(TypeInfo serviceType) => Type(serviceType, _emptyObjectArray);
        public static object Type(TypeInfo serviceType, object arg1) => Type(serviceType, new[] { arg1 });
        public static object Type(TypeInfo serviceType, object arg1, object arg2) => Type(serviceType, new[] { arg1, arg2 });
        public static object Type(TypeInfo serviceType, object arg1, object arg2, object arg3) => Type(serviceType, new[] { arg1, arg2, arg3 });
        public static object Type(TypeInfo serviceType, object arg1, object arg2, object arg3, object arg4) => Type(serviceType, new[] { arg1, arg2, arg3, arg4 });
        public static object Type(TypeInfo serviceType, object[] args)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (serviceType.IsInterface || serviceType.IsAbstract)
                throw new ArgumentException("You cannot activate a type that is marked as an interface or abstract");

            return _typeCache.GetOrAdd(serviceType, type => new TypeCacheEntry(type)).Create(ref args);
        }

        private class TypeCacheEntry
        {
            private Func<object> _fastCreator = null;
            private Ctor[] _constructors;

            internal bool HasParameterlessCtor => _fastCreator != null;

            internal TypeCacheEntry(TypeInfo type)
            {
                _constructors = type.DeclaredConstructors.Select(x => new Ctor(x)).ToArray();

                var fastCtor = _constructors.FirstOrDefault(x => x.ArgumentCount == 0).ConstructorInfo;
                if(fastCtor != null)
                    _fastCreator = Expression.Lambda<Func<object>>(Expression.New(fastCtor)).Compile();
            }

            internal object Create(ref object[] args)
            {
                if (args.Length == 0 && HasParameterlessCtor)
                    return _fastCreator();

                for(var i = 0; i < _constructors.Length; i++)
                {
                    var ctor = _constructors[i];
                    if (ctor.IsMatch(ref args))
                        return ctor.Invoke(ref args);
                }

                throw new TypeInitializationException("", null);
            }
        }

        private class Ctor
        {
            private readonly Func<object[], object> _creator;
            private TypeInfo[] _typeArguments;

            internal int ArgumentCount => _typeArguments.Length;
            internal ConstructorInfo ConstructorInfo { get; }

            internal Ctor(ConstructorInfo ctorInfo)
            {
                ConstructorInfo = ctorInfo;
                _typeArguments = ctorInfo.GetParameters().Select(x => x.ParameterType.GetTypeInfo()).ToArray();

                var array = Expression.Parameter(typeof(object[]));
                var parameters = new List<Expression>();
                for(var i = 0; i < ArgumentCount; i++)
                {
                    parameters.Add(Expression.ConvertChecked(Expression.ArrayIndex(array, Expression.Constant(i)), _typeArguments[i].AsType()));
                }

                _creator = Expression.Lambda<Func<object[], object>>(Expression.New(ctorInfo, parameters), array).Compile();
            }

            internal bool IsMatch(ref object[] args)
            {
                if (args.Length != ArgumentCount)
                    return false;

                for(var i = 0; i < ArgumentCount; i++)
                    if(!args[i].GetType().GetTypeInfo().IsAssignableFrom(_typeArguments[i]))
                        return false;

                return true;
            }
            internal object Invoke(ref object[] args) => _creator(args);            
        }
    }
}
