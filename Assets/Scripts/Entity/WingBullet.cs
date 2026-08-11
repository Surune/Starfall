using UnityEngine;
using Starfall.Manager;
using Starfall.Utils;

namespace Starfall.Entity
{
    public class WingBullet : MonoBehaviour
    {
        public static float Speed = 20f;
        public static float Damage = 1f;
        public static bool Udo = false;
        public static bool Freezing = false;

        Vector3 worldPos;

        private void Start()
        {
            Speed = 20f;
            Damage = 1f;
            Udo = false;
            Freezing = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().GetDamage(Damage, critical : false, mute : true);
                if (Freezing)
                {
                    collision.gameObject.GetComponent<Enemy>().SlowTime = 2f;
                }
                gameObject.SetActive(false);
            }
            else if (collision.transform.CompareTag("Boss"))
            {
                collision.gameObject.GetComponent<Boss>().GetDamage(Damage, critical : false);
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
                    var closest = Utility.FindClosestTransform(GameManager.Instance.EnemyList.GetAllChildren(), transform.position);
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
