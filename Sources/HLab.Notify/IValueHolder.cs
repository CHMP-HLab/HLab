using System;
using System.Linq;
using HLab.Base.Extensions;
using HLab.Notify.Triggers;

namespace HLab.Notify
{
    interface IValueHolder<T>
    {
        T Get();
        T Get(Func<T, T> valueFactory, Action<T, T> postUpdateAction = null);
        bool Set(Func<T, T> valueFactory, Action<T, T> postUpdateAction = null);
        void SetGetter(Func<T, T> valueFactory, T oldValue = default(T));
        void Reset(Action<T, T> postUpdateAction = null);
        bool HasGetter { get; }
        bool IsSet { get; }
    }

    class LazyValueHolder<T> : IValueHolder<T>
    {
        interface IBoxed
        {
            T CurrentValue { get; }
            T Value { get; }
            void Reset(Action<T, T> postUpdateAction = null);
        }

        class Boxed : IBoxed
        {
            internal Boxed(T value)
            {
                Value = value;
            }
            public T Value { get; }
            public T CurrentValue => Value;
            public void Reset(Action<T, T> postUpdateAction = null) {}
        }

        class BoxedLazy : IBoxed
        {
            private readonly Func<T,T> _valueFactory;
            private readonly LazyValueHolder<T> _parent;
            private readonly T _oldValue;
            internal BoxedLazy(LazyValueHolder<T> parent, Func<T, T> valueFactory, T oldValue)
            {
                _parent = parent;
                _valueFactory = valueFactory;
                _oldValue = oldValue;
            }

            public T Value
            {
                get
                {
                    try
                    {
                        var v = _valueFactory(_oldValue);
                        _parent._value = new BoxedLazySet(_parent, _valueFactory, v);
                        return v;
                    }
                    catch (PropertyNotReady ex)
                    {
                        return ex.ReturnValue.Cast<T>();
                    }
                    catch (NullReferenceException)
                    {
                        return default(T);
                    }
                }
            }

            public T CurrentValue => _oldValue;

            public void Reset(Action<T, T> postUpdateAction = null)
            {
                //_parent._value = new BoxedLazy(_parent, _valueFactory, Value);

                _parent.Set(_valueFactory, postUpdateAction);
            }
        }



        class BoxedLazySet : IBoxed
        {
            private readonly Func<T, T> _valueFactory;
            private readonly LazyValueHolder<T> _parent;

            internal BoxedLazySet(LazyValueHolder<T> parent, Func<T, T> valueFactory, T value)
            {
                _parent = parent;
                _valueFactory = valueFactory;
                Value = value;
            }

            public T Value { get; }

            public T CurrentValue => Value;

            public void Reset(Action<T, T> postUpdateAction = null)
            {
                //_parent._value = new BoxedLazy(_parent, _valueFactory, Value);

                _parent.Set(_valueFactory, postUpdateAction);
            }
        }


        private IBoxed _value;

        public void SetGetter(Func<T,T> valueFactory, T oldValue = default(T))
        {
            if (_value == null)
            {
                if(oldValue == null)
                    _value = new BoxedLazy(this, valueFactory, oldValue);
                else
                {
                    _value = new BoxedLazySet(this, valueFactory, oldValue);
                }

            }
        }

        public void Reset(Action<T, T> postUpdateAction = null)
        {
            _value?.Reset(postUpdateAction);
        }

        public bool HasGetter => _value != null;
        public bool IsSet => _value is BoxedLazySet;

        public T Get()
        {
            if (_value == null)
                return default(T);

            return _value.CurrentValue;
        }

        public T Get(Func<T, T> factory, Action<T, T> postUpdateAction = null)
        {
            if (_value == null) _value = new BoxedLazy(this,factory,default(T));
            return _value.Value;
        }

        public bool Set(Func<T, T> factory, Action<T, T> postUpdateAction = null)
        {
            var old = _value == null ? default(T) : _value.Value;
            var value = factory(old);

            if (Equals(old, value))
                return false;

            if (value != null && old != null && value.GetType().IsArray)
            {
                var a1 = (old as Array).Cast<object>().ToArray();
                var a2 = (value as Array).Cast<object>().ToArray();

                if (a1.Length == a2.Length)
                {
                    var eq = !a1.Where((t, i) => !Equals(t, a2[i])).Any();
                    if (eq) return false;
                }
            }

            _value = new BoxedLazySet(this,factory,value);

            postUpdateAction?.Invoke(old,value);

            return true;
        }
    }
}
