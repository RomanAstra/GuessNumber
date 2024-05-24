using System;

namespace UniRx
{
    public static class UniRxHelpers
    {
        public static IDisposable SubscribePair<T>(this IReadOnlyReactiveProperty<T> property, Action<Pair<T>> onChange, bool invokeOnStart = true)
        {
            IDisposable disposable = property.Pairwise().Subscribe(onChange);
            if (invokeOnStart)
            {
                onChange?.Invoke(new Pair<T>(default, property.Value));
            }
            return disposable;
        }
    }
}