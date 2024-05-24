using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helpers
{
    public interface IPoolObject : IDisposable
    {
        void Release();
    }
    public sealed class Pool<T> : IDisposable where T : Component, IPoolObject
    {
        private readonly Queue<T> _poolObjects;
        private readonly Transform _poolRoot;
        private readonly T _prefab;

        public Pool(T prefab)
        {
            _prefab = prefab;
            _poolRoot = new GameObject("root pool").transform;
            _poolObjects = new Queue<T>();
        }

        public T Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_poolObjects.Count == 0)
            {
                return Object.Instantiate(_prefab, position, rotation, parent);
            }

            var poolObject = _poolObjects.Dequeue();
            poolObject.transform.position = position;
            poolObject.transform.rotation = rotation;
            poolObject.transform.SetParent(parent);
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }

        public void Release(T poolObject)
        {
            poolObject.transform.SetParent(_poolRoot);
            poolObject.gameObject.SetActive(false);
            _poolObjects.Enqueue(poolObject);
            poolObject.Release();
        }

        public void Dispose()
        {
            int poolObjectsCount = _poolObjects.Count;
            for (int i = 0; i < poolObjectsCount; i++)
            {
                T dequeue = _poolObjects.Dequeue();
                dequeue.Dispose();
                Object.Destroy(dequeue.gameObject);
            }
            Object.Destroy(_poolRoot.gameObject);
        }
    }
}
