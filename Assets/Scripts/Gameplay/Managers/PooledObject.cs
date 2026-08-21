using System;
using UnityEngine;

namespace Gameplay.Managers
{
    public class PooledObject : MonoBehaviour
    {
        private IPoolable poolable;
        private PoolManager poolManager;

        public Type PoolType { get; private set; }

        public void Configure(PoolManager manager, Type poolType, Component pooledComponent)
        {
            poolManager = manager;
            PoolType = poolType;

            poolable = (IPoolable)pooledComponent;
        }

        public void NotifySpawned()
        {
            poolable.OnSpawn();
        }

        public void NotifyDespawned()
        {
            poolable.OnDespawn();
        }

        private void OnDisable()
        {
            poolManager.Return(this);
        }
    }
}
