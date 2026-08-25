using Mirror;
using UnityEngine;
using Gameplay.Managers;
using Gameplay.Projectiles;
using Gameplay.Spawning;

namespace Gameplay.Entities
{
    public class WingBullet : NetworkBehaviour, IPoolable
    {
        public static float Speed = 20f;
        public static float Damage = 1f;
        public static bool Udo = false;
        public static bool Freezing = false;

        private GameStateManager gameStateManager;
        private ProjectileNavigator navigator;
        private ProjectileTargetResolver targetResolver;

        public void Initialize(GameStateManager gameStateManager, Spawner spawner)
        {
            this.gameStateManager = gameStateManager;
            navigator = new ProjectileNavigator(spawner);
            targetResolver = new ProjectileTargetResolver();
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }

        public override void OnStartClient()
        {
            transform.SetParent(GameManager.Instance.PoolManager.EntitiesTransform);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isServer)
            {
                return;
            }

            if (targetResolver.IsObstacle(collision))
            {
                Despawn();
                return;
            }

            if (targetResolver.IsTarget(collision))
            {
                var target = targetResolver.GetDamageable(collision);
                target.ApplyDamage(Damage, mute: !target.IsBoss);
                if (Freezing)
                {
                    target.ApplySlow(2f);
                }
                Despawn();
            }
        }

        private void FixedUpdate()
        {
            if (!isServer)
            {
                return;
            }

            navigator.Move(transform, gameStateManager.IsPlaying, Speed, Udo);
        }

        private void Despawn()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
