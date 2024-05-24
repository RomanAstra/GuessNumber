using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace UniRx
{
    public interface ICovariantCollectionAddEvent<out T>
    {
        int Index { get; }
        T Value { get; }
    }

    public interface ICovariantCollectionRemoveEvent<out T>
    {
        int Index { get; }
        T Value { get; }
    }

    public interface ICovariantCollectionMoveEvent<out T>
    {
        int OldIndex { get; }
        int NewIndex { get; }
        T Value { get; }
    }

    public interface ICovariantCollectionReplaceEvent<out T> 
    {
        int Index { get; }
        T OldValue { get; }
        T NewValue { get; }
    }

    public class CovariantCollectionAddEvent<T> : ICovariantCollectionAddEvent<T>, IEquatable<CollectionAddEvent<T>>
    {
        public int Index { get; private set; }
        public T Value { get; private set; }

        public CovariantCollectionAddEvent(int index, T value)
        {
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} Value:{1}", Index, Value);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(CollectionAddEvent<T> other)
        {
            return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }

    public struct CovariantCollectionRemoveEvent<T> : ICovariantCollectionRemoveEvent<T>, IEquatable<CollectionRemoveEvent<T>>
    {
        public int Index { get; private set; }
        public T Value { get; private set; }

        public CovariantCollectionRemoveEvent(int index, T value)
            : this()
        {
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} Value:{1}", Index, Value);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(CollectionRemoveEvent<T> other)
        {
            return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }

    public struct CovariantCollectionMoveEvent<T> : ICovariantCollectionMoveEvent<T>, IEquatable<CollectionMoveEvent<T>>
    {
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
        public T Value { get; private set; }

        public CovariantCollectionMoveEvent(int oldIndex, int newIndex, T value)
            : this()
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("OldIndex:{0} NewIndex:{1} Value:{2}", OldIndex, NewIndex, Value);
        }

        public override int GetHashCode()
        {
            return OldIndex.GetHashCode() ^ NewIndex.GetHashCode() << 2 ^ EqualityComparer<T>.Default.GetHashCode(Value) >> 2;
        }

        public bool Equals(CollectionMoveEvent<T> other)
        {
            return OldIndex.Equals(other.OldIndex) && NewIndex.Equals(other.NewIndex) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }

    public struct CovariantCollectionReplaceEvent<T> : ICovariantCollectionReplaceEvent<T>, IEquatable<CollectionReplaceEvent<T>>
    {
        public int Index { get; private set; }
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public CovariantCollectionReplaceEvent(int index, T oldValue, T newValue)
            : this()
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} OldValue:{1} NewValue:{2}", Index, OldValue, NewValue);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(OldValue) << 2 ^ EqualityComparer<T>.Default.GetHashCode(NewValue) >> 2;
        }

        public bool Equals(CollectionReplaceEvent<T> other)
        {
            return Index.Equals(other.Index)
                   && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue)
                   && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
        }
    }

    // IReadOnlyList<out T> is from .NET 4.5
    public interface IReadOnlyCovariantReactiveCollection<out T> : IReadOnlyList<T>
    {
        int Count { get; }
        T this[int index] { get; }
        IObservable<ICovariantCollectionAddEvent<T>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<ICovariantCollectionMoveEvent<T>> ObserveMove();
        IObservable<ICovariantCollectionRemoveEvent<T>> ObserveRemove();
        IObservable<ICovariantCollectionReplaceEvent<T>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }

    public interface ICovariantReactiveCollection<T> : IList<T>, IReadOnlyCovariantReactiveCollection<T>
    {
        new int Count { get; }
        new T this[int index] { get; set; }
        void Move(int oldIndex, int newIndex);
        void Sort();
        void Sort(Comparison<T> comparison);
    }

    [Serializable]
    public class CovariantReactiveCollection<T> : Collection<T>, ICovariantReactiveCollection<T>, IDisposable
    {
        [NonSerialized] bool isDisposed = false;

        public CovariantReactiveCollection() { }

        public CovariantReactiveCollection(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public CovariantReactiveCollection(List<T> list)
            : base(list != null ? new List<T>(list) : null) { }

        protected override void ClearItems()
        {
            var beforeCount = Count;
            base.ClearItems();

            if (collectionReset != null) collectionReset.OnNext(Unit.Default);
            if (beforeCount > 0)
            {
                if (countChanged != null) countChanged.OnNext(Count);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (collectionAdd != null) collectionAdd.OnNext(new CovariantCollectionAddEvent<T>(index, item));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        public void AddSilently(T item)
        {
            base.InsertItem(Count,item);
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        public void Sort()
        {
            var list = Items.ToList();
            list.Sort();
            for (var i = 0; i < list.Count; i++)
            {
                var oldIndex = Items.IndexOf(list[i]);
                MoveItem(oldIndex, i);
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            var list = Items.ToList();
            list.Sort(comparison);
            for (var i = 0; i < list.Count; i++)
            {
                var oldIndex = Items.IndexOf(list[i]);
                MoveItem(oldIndex, i);
            }
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            T item = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);

            if (collectionMove != null) collectionMove.OnNext(new CovariantCollectionMoveEvent<T>(oldIndex, newIndex, item));
        }

        protected override void RemoveItem(int index)
        {
            T item = this[index];
            base.RemoveItem(index);

            if (collectionRemove != null) collectionRemove.OnNext(new CovariantCollectionRemoveEvent<T>(index, item));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];
            base.SetItem(index, item);

            if (collectionReplace != null) collectionReplace.OnNext(new CovariantCollectionReplaceEvent<T>(index, oldItem, item));
        }


        [NonSerialized] Subject<int> countChanged = null;

        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            if (isDisposed) return Observable.Empty<int>();

            var subject = countChanged ?? (countChanged = new Subject<int>());
            if (notifyCurrentCount)
            {
                return subject.StartWith(() => this.Count);
            }
            else
            {
                return subject;
            }
        }

        [NonSerialized] Subject<Unit> collectionReset = null;

        public IObservable<Unit> ObserveReset()
        {
            if (isDisposed) return Observable.Empty<Unit>();
            return collectionReset ?? (collectionReset = new Subject<Unit>());
        }

        [NonSerialized] Subject<ICovariantCollectionAddEvent<T>> collectionAdd = null;

        public IObservable<ICovariantCollectionAddEvent<T>> ObserveAdd()
        {
            if (isDisposed) return Observable.Empty<ICovariantCollectionAddEvent<T>>();
            return collectionAdd ?? (collectionAdd = new Subject<ICovariantCollectionAddEvent<T>>());
        }

        [NonSerialized] Subject<ICovariantCollectionMoveEvent<T>> collectionMove = null;

        public IObservable<ICovariantCollectionMoveEvent<T>> ObserveMove()
        {
            if (isDisposed) return Observable.Empty<ICovariantCollectionMoveEvent<T>>();
            return collectionMove ?? (collectionMove = new Subject<ICovariantCollectionMoveEvent<T>>());
        }

        [NonSerialized] Subject<ICovariantCollectionRemoveEvent<T>> collectionRemove = null;

        public IObservable<ICovariantCollectionRemoveEvent<T>> ObserveRemove()
        {
            if (isDisposed) return Observable.Empty<ICovariantCollectionRemoveEvent<T>>();
            return collectionRemove ?? (collectionRemove = new Subject<ICovariantCollectionRemoveEvent<T>>());
        }

        [NonSerialized] Subject<ICovariantCollectionReplaceEvent<T>> collectionReplace = null;

        public IObservable<ICovariantCollectionReplaceEvent<T>> ObserveReplace()
        {
            if (isDisposed) return Observable.Empty<ICovariantCollectionReplaceEvent<T>>();
            return collectionReplace ?? (collectionReplace = new Subject<ICovariantCollectionReplaceEvent<T>>());
        }

        void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
        {
            if (subject != null)
            {
                try
                {
                    subject.OnCompleted();
                }
                finally
                {
                    subject.Dispose();
                    subject = null;
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeSubject(ref collectionReset);
                    DisposeSubject(ref collectionAdd);
                    DisposeSubject(ref collectionMove);
                    DisposeSubject(ref collectionRemove);
                    DisposeSubject(ref collectionReplace);
                    DisposeSubject(ref countChanged);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    public static partial class ReactiveCollectionExtensions
    {
        public static CovariantReactiveCollection<T> ToCovariantReactiveCollection<T>(this IEnumerable<T> source)
        {
            return new CovariantReactiveCollection<T>(source);
        }
    }
}