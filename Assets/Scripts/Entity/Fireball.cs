using UnityEngine;
using Starfall.Manager;

namespace Starfall.Entity
{
    public class Fireball : MonoBehaviour
    {
        static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;

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

        Vector3 worldPos;

        void OnEnable()
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

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                if (IsFatal)
                {
                    if (collision.gameObject.GetComponent<Enemy>().IsBoss == false)
                    {
                        collision.gameObject.GetComponent<Enemy>().GetDamage(Damage, fatal : true);
                    }
                    else
                    {
                        collision.gameObject.GetComponent<Enemy>().GetDamage(Damage * FatalDamage, fatal : false);
                    }
                }
                else if (Psychosink)
                {
                    PlayerManager.DamageAllEnemy(Damage);
                }
                else
                {
                    collision.gameObject.GetComponent<Enemy>().GetDamage(Damage, critical : IsCritical);
                }

                if (IsCritical && Burst)
                {
                    collision.gameObject.GetComponent<Enemy>().GetDamage(PlayerManager.damage, mute: true);
                }
                if (Freezing)
                {
                    collision.gameObject.GetComponent<Enemy>().SlowTime = 2f;
                }
                if (!Penetrate)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    Penetrate = false;
                }
            }
            else if (collision.transform.CompareTag("Boss"))
            {
                if (IsFatal)
                {
                    Damage *= FatalDamage;
                }
                collision.gameObject.GetComponent<Boss>().GetDamage(Damage, IsCritical);

                if (IsCritical && Burst)
                {
                    collision.gameObject.GetComponent<Boss>().GetDamage(PlayerManager.damage, mute: true);
                }
                if (Freezing)
                {
                    collision.gameObject.GetComponent<Boss>().SlowTime = 2f;
                }
                gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            worldPos = Camera.main.WorldToViewportPoint(transform.position);
            if (worldPos.x < 0f || worldPos.x > 1f || worldPos.y < 0f || worldPos.y > 1f)
            {
                gameObject.SetActive(false);
            }
            else if (GameStateManager.Instance.IsPlaying)
            {
                if (Udo)
                {
                    Transform closest = GameManager.FindClosestTransform(GameManager.GetAllChilds(GameManager.Instance.EnemyList), transform.position);
                    if (closest != null && Vector2.Distance(transform.position, closest.position) < 1f)
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
