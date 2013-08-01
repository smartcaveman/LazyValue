using System;

namespace LazyValue
{
    public static class Lazy
    {
        public static LazyEquatable<T> Equatable<T>(this T obj)
        {
            return Lazy.Equatable(() => obj);
        }

        public static LazyEquatable<T> Equatable<T>(this Func<T> func)
        {
            return ReferenceEquals(func, null) ? LazyEquatable<T>.Default : new LazyEquatable<T>(func);
        }

        public static LazyEquatable<T> Equatable<T>(this Lazy<T> lazyObj)
        {
            if (ReferenceEquals(null, lazyObj))
            {
                return LazyEquatable<T>.Default;
            }
            var lazyEquatable = lazyObj as LazyEquatable<T>;
            return !ReferenceEquals(null, lazyEquatable)
                       ? lazyEquatable
                       : (lazyObj.IsValueCreated
                              ? (LazyEquatable<T>.Default.Equals(lazyObj)
                                     ? LazyEquatable<T>.Default
                                     : new LazyEquatable<T>(lazyObj.Value))
                              : new LazyEquatable<T>(() => lazyObj.Value));
        }
    }
}