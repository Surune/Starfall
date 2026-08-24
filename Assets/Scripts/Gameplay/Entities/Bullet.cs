using UnityEngine;
using Gameplay.Managers;
using Gameplay.Projectiles;
using Gameplay.Spawning;

namespace Gameplay.Entities
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        private PlayerManager playerManager;
        private GameStateManager gameStateManager;
        private ProjectileNavigator navigator;
        private ProjectileTargetResolver targetResolver;

        public static float FatalDamage = 2f;

        public float Speed = 20f;
        public float Damage = 1f;
        public bool Udo = false;
        public bool IsCritical = false;
        public bool Penetrate = false;
        public bool IsFatal = false;
        public bool Psychosink = false;
        public bool Burst = false;
        public bool Freezing = false;

        public void Initialize(PlayerManager playerManager, GameStateManager gameStateManager, Spawner spawner)
        {
            this.playerManager = playerManager;
            this.gameStateManager = gameStateManager;
            navigator = new ProjectileNavigator(spawner);
            targetResolver = new ProjectileTargetResolver();
        }

        public void OnSpawn()
        {
            Udo = false;
            IsCritical = false;
            Penetrate = false;
            IsFatal = false;
            Psychosink = false;
            Burst = false;
            Freezing = false;
        }

        public void OnDespawn()
        {
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
                if (IsFatal)
                {
                    target.ApplyDamage(target.IsBoss ? Damage * FatalDamage : Damage, fatal : !target.IsBoss);
                }
                else if (Psychosink && !target.IsBoss)
                {
                    playerManager.DamageAllEnemy(Damage);
                }
                else
                {
                    target.ApplyDamage(Damage, critical : IsCritical);
                }

                if (IsCritical && Burst)
                {
                    target.ApplyDamage(playerManager.damage, mute: true);
                }
                if (Freezing)
                {
                    target.ApplySlow(2f);
                }
                if (!Penetrate || target.IsBoss)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    Penetrate = false;
                }
            }
        }

        private void FixedUpdate()
        {
            navigator.Move(transform, gameStateManager.IsPlaying, Speed, Udo);
        }
    }
}
