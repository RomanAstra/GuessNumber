using System;
using System.Collections.Generic;

namespace Helpers
{
    public sealed class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> _stack = new Stack<T>();
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnRelease;

        public bool IsEmpty => _stack.Count == 0;
        private readonly bool _createIfEmpty;

        public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease, bool createIfEmpty = true)
        {
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _createIfEmpty = createIfEmpty;
        }

        public T Get()
        {
            T element;
            if (_stack.Count == 0)
            {
                if (_createIfEmpty)
                {
                    element = new T();
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                element = _stack.Pop();
            }

            _actionOnGet?.Invoke(element);
            return element;
        }

        public void Release(T element)
        {
            _actionOnRelease?.Invoke(element);
            _stack.Push(element);
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}
