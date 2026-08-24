using UnityEngine;
using Gameplay.Managers;
using Gameplay.Projectiles;
using Gameplay.Spawning;

namespace Gameplay.Entities
{
    public class WingBullet : MonoBehaviour, IPoolable
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

        private void Start()
        {
            Speed = 20f;
            Damage = 1f;
            Udo = false;
            Freezing = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (targetResolver.IsObstacle(collision))
            {
                gameObject.SetActive(false);
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
                gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            navigator.Move(transform, gameStateManager.IsPlaying, Speed, Udo);
        }
    }
}
