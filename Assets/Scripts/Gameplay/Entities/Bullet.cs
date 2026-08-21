using UnityEngine;
using Gameplay.Managers;
using Gameplay.Spawning;

namespace Gameplay.Entities
{
    public class Bullet : MonoBehaviour, IDependencyInjectable, IPoolable
    {
        private PlayerManager playerManager;
        private Spawner spawner;
        private GameStateManager gameStateManager;

        public static float FatalDamage = 2f;

        public float Speed = 20f;
        public float Damage = 1f;
        public bool Udo = false;
        public bool IsCritical = false;
        public bool Penetrate = false;
        public bool IsFatal = false;
        public bool Psychosink = false;
        public bool Beingstronger = false;
        public bool Burst = false;
        public bool Freezing = false;

        private Vector3 worldPos;

        public void InjectDependency(GameDependencies dependencies)
        {
            playerManager = dependencies.PlayerManager;
            spawner = dependencies.Spawner;
            gameStateManager = dependencies.GameStateManager;
        }

        public void OnSpawn()
        {
            Udo = false;
            IsCritical = false;
            Penetrate = false;
            IsFatal = false;
            Psychosink = false;
            Beingstronger = false;
            Burst = false;
            Freezing = false;
        }

        public void OnDespawn()
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Boss"))
            {
                var target = collision.gameObject.GetComponent<IDamageable>();
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
            worldPos = Camera.main.WorldToViewportPoint(transform.position);
            if (worldPos.x < 0f || worldPos.x > 1f || worldPos.y < 0f || worldPos.y > 1f)
            {
                gameObject.SetActive(false);
            }
            else if (gameStateManager.IsPlaying)
            {
                if (Udo)
                {
                    var closest = spawner.FindClosestTarget(transform.position);
                    if (closest && Vector2.Distance(transform.position, closest.position) < 1f)
                    {
                        transform.position = Vector3.Lerp(transform.position, closest.position, Time.smoothDeltaTime * Speed);
                    }
                    else
                    {
                        transform.Translate(0, Time.smoothDeltaTime * Speed, 0);
                    }
                }
                else
                {
                    transform.Translate(0, Time.smoothDeltaTime * Speed, 0);
                }

                if (Beingstronger)
                {
                    Damage += Damage * Time.smoothDeltaTime;
                }
            }
        }
    }
}
