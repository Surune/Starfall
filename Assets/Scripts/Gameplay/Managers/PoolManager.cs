using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Gameplay.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public GameObject[] Prefabs;
        public Transform EntitiesTransform;
        private Dictionary<Type, Queue<PooledObject>> availableObjects;
        private Dictionary<Type, int> prefabIndexes;
        private Action<Component> objectInitializer;

        private void Awake()
        {
            availableObjects = new Dictionary<Type, Queue<PooledObject>>();
            prefabIndexes = new Dictionary<Type, int>();

            for (var index = 0; index < Prefabs.Length; index++)
            {
                var poolType = GetPoolType(Prefabs[index]);
                availableObjects.Add(poolType, new Queue<PooledObject>());
                prefabIndexes.Add(poolType, index);
            }
        }

        public void SetObjectInitializer(Action<Component> initializer)
        {
            objectInitializer = initializer;
        }

        public T Spawn<T>() where T : Component
        {
            return Spawn<T>(_ => { });
        }

        public T Spawn<T>(Action<T> configure) where T : Component
        {
            var poolType = typeof(T);
            if (Prefabs[prefabIndexes[poolType]].TryGetComponent<NetworkIdentity>(out _))
            {
                var networkObject = Instantiate(Prefabs[prefabIndexes[poolType]], EntitiesTransform);
                var networkComponent = networkObject.GetComponent<T>();
                objectInitializer(networkComponent);
                ((IPoolable)networkComponent).OnSpawn();
                configure(networkComponent);
                NetworkServer.Spawn(networkObject);
                return networkComponent;
            }

            var available = availableObjects[poolType];
            var pooledObject = available.Count > 0
                ? available.Dequeue()
                : Create(poolType);

            pooledObject.gameObject.SetActive(true);
            pooledObject.NotifySpawned();
            var pooledComponent = pooledObject.GetComponent<T>();
            configure(pooledComponent);
            return pooledComponent;
        }

        public void Release(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<NetworkIdentity>(out _))
            {
                NetworkServer.Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
        }

        internal void Return(PooledObject pooledObject)
        {
            pooledObject.NotifyDespawned();
            availableObjects[pooledObject.PoolType].Enqueue(pooledObject);
        }

        private PooledObject Create(Type poolType)
        {
            var index = prefabIndexes[poolType];
            var gameObject = Instantiate(Prefabs[index], EntitiesTransform);
            var pooledComponent = gameObject.GetComponent(poolType);
            var pooledObject = gameObject.AddComponent<PooledObject>();
            pooledObject.Configure(this, poolType, pooledComponent);
            objectInitializer(pooledComponent);
            return pooledObject;
        }

        private Type GetPoolType(GameObject prefab)
        {
            foreach (var behaviour in prefab.GetComponents<MonoBehaviour>())
            {
                if (behaviour is IPoolable)
                {
                    return behaviour.GetType();
                }
            }

            throw new InvalidOperationException($"{prefab.name}에 IPoolable 컴포넌트가 없습니다.");
        }
    }
}
