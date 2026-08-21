using UnityEngine;
using Gameplay.Managers;
using Gameplay.Spawning;

namespace Gameplay.Entities
{
    public class WingBullet : MonoBehaviour, IDependencyInjectable, IPoolable
    {
        public static float Speed = 20f;
        public static float Damage = 1f;
        public static bool Udo = false;
        public static bool Freezing = false;

        private Spawner spawner;
        private GameStateManager gameStateManager;

        Vector3 worldPos;

        public void InjectDependency(GameDependencies dependencies)
        {
            spawner = dependencies.Spawner;
            gameStateManager = dependencies.GameStateManager;
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
            if (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Boss"))
            {
                var target = collision.gameObject.GetComponent<IDamageable>();
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
            }
        }
    }
}
