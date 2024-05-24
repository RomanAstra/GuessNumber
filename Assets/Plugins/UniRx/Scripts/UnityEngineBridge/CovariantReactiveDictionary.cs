using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UniRx
{
    
    public interface IDictionaryAddEvent<out TKey, out TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
    }
    
    public interface IDictionaryRemoveEvent<out TKey, out TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
    }
    
    public interface IDictionaryReplaceEvent<out TKey, out TValue>
    {
        TKey Key { get; } 
        TValue OldValue { get; }
        TValue NewValue { get; }
    }
    
    public struct CovariantDictionaryAddEvent<TKey, TValue> : IDictionaryAddEvent<TKey, TValue>, IEquatable<DictionaryAddEvent<TKey, TValue>>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public CovariantDictionaryAddEvent(TKey key, TValue value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Key:{0} Value:{1}", Key, Value);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ EqualityComparer<TValue>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(DictionaryAddEvent<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
    }

    public struct CovariantDictionaryRemoveEvent<TKey, TValue> : IDictionaryRemoveEvent<TKey, TValue>, IEquatable<DictionaryRemoveEvent<TKey, TValue>>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public CovariantDictionaryRemoveEvent(TKey key, TValue value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Key:{0} Value:{1}", Key, Value);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ EqualityComparer<TValue>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(DictionaryRemoveEvent<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
    }

    public struct CovariantDictionaryReplaceEvent<TKey, TValue> : IDictionaryReplaceEvent<TKey, TValue>, IEquatable<DictionaryReplaceEvent<TKey, TValue>>
    {
        public TKey Key { get; private set; }
        public TValue OldValue { get; private set; }
        public TValue NewValue { get; private set; }

        public CovariantDictionaryReplaceEvent(TKey key, TValue oldValue, TValue newValue)
            : this()
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            return string.Format("Key:{0} OldValue:{1} NewValue:{2}", Key, OldValue, NewValue);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ EqualityComparer<TValue>.Default.GetHashCode(OldValue) << 2 ^ EqualityComparer<TValue>.Default.GetHashCode(NewValue) >> 2;
        }

        public bool Equals(DictionaryReplaceEvent<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<TValue>.Default.Equals(NewValue, other.NewValue);
        }
    }

    // IReadOnlyDictionary is from .NET 4.5
    public interface IReadOnlyCovariantReactiveDictionary<TKey, out TValue> 
    {
        int Count { get; }
        TValue this[TKey index] { get; }
        bool ContainsKey(TKey key);

        IObservable<IDictionaryAddEvent<TKey, TValue>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<IDictionaryRemoveEvent<TKey, TValue>> ObserveRemove();
        IObservable<IDictionaryReplaceEvent<TKey, TValue>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }

    public interface ICovariantReactiveDictionary<TKey, TValue> : IReadOnlyCovariantReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
    }

    [Serializable]
    public class CovariantReactiveDictionary<TKey, TValue> : ICovariantReactiveDictionary<TKey, TValue>, IDictionary, IDisposable
#if !UNITY_METRO
                                                           , ISerializable, IDeserializationCallback
#endif
    {
        [NonSerialized]
        bool isDisposed = false;

#if !UniRxLibrary
        [UnityEngine.SerializeField]
#endif
        readonly Dictionary<TKey, TValue> inner;

        public CovariantReactiveDictionary()
        {
            inner = new Dictionary<TKey, TValue>();
        }

        public CovariantReactiveDictionary(IEqualityComparer<TKey> comparer)
        {
            inner = new Dictionary<TKey, TValue>(comparer);
        }

        public CovariantReactiveDictionary(Dictionary<TKey, TValue> innerDictionary)
        {
            inner = innerDictionary;
        }

        public TValue this[TKey key]
        {
            get
            {
                return inner[key];
            }

            set
            {
                TValue oldValue;
                if (TryGetValue(key, out oldValue))
                {
                    inner[key] = value;
                    if (dictionaryReplace != null) dictionaryReplace.OnNext(new CovariantDictionaryReplaceEvent<TKey, TValue>(key, oldValue, value));
                }
                else
                {
                    inner[key] = value;
                    if (dictionaryAdd != null) dictionaryAdd.OnNext(new CovariantDictionaryAddEvent<TKey, TValue>(key, value));
                    if (countChanged != null) countChanged.OnNext(Count);
                }
            }
        }

        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                return inner.Keys;
            }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                return inner.Values;
            }
        }

        public void Add(TKey key, TValue value)
        {
            inner.Add(key, value);

            if (dictionaryAdd != null) dictionaryAdd.OnNext(new CovariantDictionaryAddEvent<TKey, TValue>(key, value));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        public void Clear()
        {
            var beforeCount = Count;
            inner.Clear();

            if (collectionReset != null) collectionReset.OnNext(Unit.Default);
            if (beforeCount > 0)
            {
                if (countChanged != null) countChanged.OnNext(Count);
            }
        }

        public bool Remove(TKey key)
        {
            TValue oldValue;
            if (inner.TryGetValue(key, out oldValue))
            {
                var isSuccessRemove = inner.Remove(key);
                if (isSuccessRemove)
                {
                    if (dictionaryRemove != null) dictionaryRemove.OnNext(new CovariantDictionaryRemoveEvent<TKey, TValue>(key, oldValue));
                    if (countChanged != null) countChanged.OnNext(Count);
                }
                return isSuccessRemove;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return inner.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return inner.TryGetValue(key, out value);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return inner.GetEnumerator();
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
                    DisposeSubject(ref countChanged);
                    DisposeSubject(ref collectionReset);
                    DisposeSubject(ref dictionaryAdd);
                    DisposeSubject(ref dictionaryRemove);
                    DisposeSubject(ref dictionaryReplace);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion


        #region Observe

        [NonSerialized]
        Subject<int> countChanged = null;
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

        [NonSerialized]
        Subject<Unit> collectionReset = null;
        public IObservable<Unit> ObserveReset()
        {
            if (isDisposed) return Observable.Empty<Unit>();
            return collectionReset ?? (collectionReset = new Subject<Unit>());
        }

        [NonSerialized]
        Subject<IDictionaryAddEvent<TKey, TValue>> dictionaryAdd = null;
        public IObservable<IDictionaryAddEvent<TKey, TValue>> ObserveAdd()
        {
            if (isDisposed) return Observable.Empty<IDictionaryAddEvent<TKey, TValue>>();
            return dictionaryAdd ?? (dictionaryAdd = new Subject<IDictionaryAddEvent<TKey, TValue>>());
        }

        [NonSerialized]
        Subject<IDictionaryRemoveEvent<TKey, TValue>> dictionaryRemove = null;
        public IObservable<IDictionaryRemoveEvent<TKey, TValue>> ObserveRemove()
        {
            if (isDisposed) return Observable.Empty<IDictionaryRemoveEvent<TKey, TValue>>();
            return dictionaryRemove ?? (dictionaryRemove = new Subject<IDictionaryRemoveEvent<TKey, TValue>>());
        }

        [NonSerialized]
        Subject<IDictionaryReplaceEvent<TKey, TValue>> dictionaryReplace = null;
        public IObservable<IDictionaryReplaceEvent<TKey, TValue>> ObserveReplace()
        {
            if (isDisposed) return Observable.Empty<IDictionaryReplaceEvent<TKey, TValue>>();
            return dictionaryReplace ?? (dictionaryReplace = new Subject<IDictionaryReplaceEvent<TKey, TValue>>());
        }

        #endregion

        #region implement explicit

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }

            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }


        bool IDictionary.IsFixedSize
        {
            get
            {
                return ((IDictionary)inner).IsFixedSize;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return ((IDictionary)inner).IsReadOnly;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((IDictionary)inner).IsSynchronized;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return ((IDictionary)inner).Keys;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((IDictionary)inner).SyncRoot;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return ((IDictionary)inner).Values;
            }
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)inner).IsReadOnly;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return inner.Keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return inner.Values;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)inner).Contains(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)inner).CopyTo(array, index);
        }

#if !UNITY_METRO

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)inner).GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            ((IDeserializationCallback)inner).OnDeserialization(sender);
        }

#endif

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add((TKey)item.Key, (TValue)item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)inner).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)inner).CopyTo(array, arrayIndex);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)inner).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue v;
            if (TryGetValue(item.Key, out v))
            {
                if (EqualityComparer<TValue>.Default.Equals(v, item.Value))
                {
                    Remove(item.Key);
                    return true;
                }
            }

            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)inner).GetEnumerator();
        }

        #endregion
    }

    public static partial class ReactiveDictionaryExtensions
    {
        public static CovariantReactiveDictionary<TKey, TValue> ToCovariantReactiveDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return new CovariantReactiveDictionary<TKey, TValue>(dictionary);
        }
    }
}
