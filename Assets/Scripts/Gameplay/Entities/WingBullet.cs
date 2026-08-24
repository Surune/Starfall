using UnityEngine;
using Gameplay.Managers;
using Gameplay.Projectiles;

namespace Gameplay.Entities
{
    public class WingBullet : MonoBehaviour, IDependencyInjectable, IPoolable
    {
        public static float Speed = 20f;
        public static float Damage = 1f;
        public static bool Udo = false;
        public static bool Freezing = false;

        private GameStateManager gameStateManager;
        private ProjectileNavigator navigator;
        private ProjectileTargetResolver targetResolver;

        public void InjectDependency(GameDependencies dependencies)
        {
            gameStateManager = dependencies.GameStateManager;
            navigator = new ProjectileNavigator(dependencies.Spawner);
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
