using System;
using System.Collections.Generic;
using System.Threading;

namespace LazyValue
{
    public class LazyEquatable<T> : Lazy<T>, IEquatable<T>, IEquatable<Lazy<T>>
    {
        static LazyEquatable()
        {
            IsValueTypeAdapter = typeof(T).IsValueType;
            _default = new LazyEquatable<T>(default(T));
        }

        private static readonly bool IsValueTypeAdapter;
        private static readonly LazyEquatable<T> _default;

        public static LazyEquatable<T> Default { get { return _default; } }

        private readonly IEqualityComparer<T> equalityComparer;

        public LazyEquatable(Func<T> valueFactory)
            : base(valueFactory) { }

        public LazyEquatable(Func<T> valueFactory, IEqualityComparer<T> equalityComparer)
            : base(valueFactory)
        {
            this.equalityComparer = equalityComparer;
        }

        public LazyEquatable(Func<T> valueFactory, bool isThreadSafe = true, IEqualityComparer<T> equalityComparer = null)
            : base(valueFactory, isThreadSafe)
        {
            this.equalityComparer = equalityComparer;
        }

        public LazyEquatable(Func<T> valueFactory, LazyThreadSafetyMode mode = LazyThreadSafetyMode.ExecutionAndPublication, IEqualityComparer<T> equalityComparer = null)
            : base(valueFactory, mode)
        {
            this.equalityComparer = equalityComparer;
        }

        public LazyEquatable(T value)
            : this(() => value) { }

        public LazyEquatable(T value, IEqualityComparer<T> equalityComparer)
            : this(() => value, equalityComparer) { }

        public LazyEquatable(T value, bool isThreadSafe = true, IEqualityComparer<T> equalityComparer = null)
            : this(() => value, isThreadSafe, equalityComparer) { }

        public LazyEquatable(T value, LazyThreadSafetyMode mode = LazyThreadSafetyMode.ExecutionAndPublication, IEqualityComparer<T> equalityComparer = null)
            : this(() => value, mode, equalityComparer) { }

        public IEqualityComparer<T> EqualityComparer
        {
            get { return equalityComparer ?? EqualityComparer<T>.Default; }
        }

        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (ReferenceEquals(obj, null))
                return !IsValueTypeAdapter && this.Equals(default(T));
            if (obj is T)
                return this.Equals((T)obj);
            return this.Equals(obj as Lazy<T>);
        }

        public bool Equals(T other)
        {
            return EqualityComparer.Equals(Value, other);
        }

        public bool Equals(Lazy<T> other)
        {
            return !ReferenceEquals(other, null) && EqualityComparer.Equals(Value, other.Value);
        }

        public sealed override int GetHashCode()
        {
            return EqualityComparer.GetHashCode(Value);
        }

        public sealed override string ToString()
        {
            return ReferenceEquals(Value, null) ? string.Empty : Value.ToString();
        }

        public static bool operator ==(LazyEquatable<T> lazyObj, object obj)
        {
            return ReferenceEquals(lazyObj, null) ? ReferenceEquals(obj, null) : lazyObj.Equals(obj);
        }

        public static bool operator !=(LazyEquatable<T> lazyObj, object obj)
        {
            return !(lazyObj == obj);
        }

        public static bool operator ==(object obj, LazyEquatable<T> lazyObj)
        {
            return ReferenceEquals(lazyObj, null) ? ReferenceEquals(obj, null) : lazyObj.Equals(obj);
        }

        public static bool operator !=(object obj, LazyEquatable<T> lazyObj)
        {
            return !(lazyObj == obj);
        }

        public static implicit operator T(LazyEquatable<T> lazyObj)
        {
            return lazyObj.Value;
        }

        public static implicit operator Func<T>(LazyEquatable<T> lazyObj)
        {
            return () => lazyObj.Value;
        }

        public static implicit operator LazyEquatable<T>(T value)
        {
            return new LazyEquatable<T>(value);
        }

        public static implicit operator LazyEquatable<T>(Func<T> valueFactory)
        {
            return new LazyEquatable<T>(valueFactory ?? (() => default(T)));
        }
    }
}