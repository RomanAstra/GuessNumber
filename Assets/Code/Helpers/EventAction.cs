using System;
using System.Collections.Generic;

namespace Helpers
{
    public interface IEventAction<out T>
    {
        IDisposable Subscribe(Action<T> action);
    }

    public sealed class EventAction<T> : IEventAction<T>, IDisposable
    {
        private sealed class ActionHolder : IDisposable
        {
            private readonly EventAction<T> _eventAction;

            private Action<T> _action;
            private bool _isDisposed;

            public ActionHolder(Action<T> action, EventAction<T> eventAction)
            {
                _action = action;
                _eventAction = eventAction;
            }

            public void Invoke(T value)
            {
                if (_isDisposed) return;
                _action.Invoke(value);
            }

            public void Dispose()
            {
                _isDisposed = true;
                _action = null;
                _eventAction.RemoveAction(this);
            }
        }

        private bool _isDisposed;
        private readonly List<ActionHolder> _actions = new();

        public IDisposable Subscribe(Action<T> action)
        {
            if (_isDisposed)
            {
                return EmptyDisposable.Singleton;
            }

            ActionHolder holder = new(action, this);
            _actions.Add(holder);
            return holder;
        }

        private readonly List<ActionHolder> _removeActions = new();

        private void RemoveAction(ActionHolder holder)
        {
            if (_isInvoking)
            {
                _removeActions.Add(holder);
                return;
            }

            _actions.Remove(holder);
        }

        private bool _isInvoking;

        public void Invoke(T value)
        {
            _isInvoking = true;
            for (var i = 0; i < _actions.Count; i++)
            {
                _actions[i].Invoke(value);
                if(_isDisposed)
                    return;
            }

            for (var i = _removeActions.Count - 1; i >= 0; --i)
            {
                _actions.Remove(_removeActions[i]);
            }

            _isInvoking = false;
            _removeActions.Clear();
        }

        public void ClearSubscribers()
        {
            for (var i = 0; i < _actions.Count; i++)
            {
                _actions[i].Dispose();
            }

            _actions.Clear();
        }

        public void Dispose()
        {
            for (var i = 0; i < _actions.Count; i++)
            {
                _actions[i].Dispose();
            }
            _isDisposed = true;
            _isInvoking = false;
            _actions.Clear();
            _removeActions.Clear();
        }
    }

    public interface IActionHolder : IDisposable
    {
    }

    public sealed class EventAction : IDisposable
    {
        private sealed class ActionHolder : IDisposable
        {
            private readonly EventAction _eventAction;

            private Action _action;
            private bool _isDisposed;

            public ActionHolder(Action action, EventAction eventAction)
            {
                _action = action;
                _eventAction = eventAction;
            }
            
            public bool HasAction(Action action)
            {
                return _action == action;
            }

            public void Invoke()
            {
                if (_isDisposed) return;
                _action.Invoke();
            }

            public void Dispose()
            {
                _isDisposed = true;
                _action = null;
                _eventAction.RemoveAction(this);
            }
        }

        private bool _isDisposed;
        private readonly List<ActionHolder> _actions = new();

        public static EventAction operator + (EventAction eventAction, Action action)
        {
            eventAction.Subscribe(action);
            return eventAction;
        }
        
        public static EventAction operator - (EventAction eventAction, Action action)
        {
            for (var i = 0; i < eventAction._actions.Count; i++)
            {
                ActionHolder holder = eventAction._actions[i];
                if (holder.HasAction(action))
                {
                    eventAction.RemoveAction(holder);
                }
            }
            return eventAction;
        }
        
        public IDisposable Subscribe(Action action)
        {
            if (_isDisposed)
            {
                throw new Exception("Can't subscribe on disposed EventAction!");
            }

            ActionHolder holder = new(action, this);
            _actions.Add(holder);
            return holder;
        }

        private readonly List<ActionHolder> _removeActions = new();

        private void RemoveAction(ActionHolder holder)
        {
            if (_isInvoking)
            {
                _removeActions.Add(holder);
                return;
            }

            _actions.Remove(holder);
        }

        private bool _isInvoking;

        public void Invoke()
        {
            _isInvoking = true;
            for (var i = 0; i < _actions.Count; i++)
            {
                _actions[i].Invoke();
            }

            for (var i = _removeActions.Count - 1; i >= 0; --i)
            {
                _actions.Remove(_removeActions[i]);
            }

            _isInvoking = false;
            _removeActions.Clear();
        }

        public void ClearSubscribers()
        {
            for (var i = 0; i < _actions.Count; i++)
            {
                _actions[i].Dispose();
            }

            _actions.Clear();
        }

        public void Dispose()
        {
            for (var i = 0; i < _actions.Count; i++)
            {
                _actions[i].Dispose();
            }

            _isDisposed = true;
        }
    }
}
