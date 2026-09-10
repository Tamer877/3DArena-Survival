using System.Collections.Generic;
using UnityEngine;

namespace ArenaSurvival.Core
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parentTransform;
        private readonly Queue<T> _poolQueue = new Queue<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parentTransform = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private T CreateNewObject()
        {
            T newObj = Object.Instantiate(_prefab, _parentTransform);
            newObj.gameObject.SetActive(false);
            _poolQueue.Enqueue(newObj);
            return newObj;
        }

        public T Get()
        {
            T obj = _poolQueue.Count > 0 ? _poolQueue.Dequeue() : CreateNewObject();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _poolQueue.Enqueue(obj);
        }
    }
}