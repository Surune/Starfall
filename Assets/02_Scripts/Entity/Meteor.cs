using UnityEngine;
using Starfall.Manager;

namespace Starfall.Entity
{
    public class Meteor : MonoBehaviour
    {
        static HPManager HpManager => GameManager.Instance.HPManager;
        public float Speed = 10f;
        Vector3 moveDirection = Vector3.down;
        int damage;

        void OnEnable()
        {
            damage = GameManager.Instance.Timer.WaveNum;
        }

        void Update()
        {
            if (gameObject.activeSelf && GameStateManager.Instance.IsPlaying)
            {
                transform.position += moveDirection * (Speed * Time.deltaTime);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                HpManager.GetDamage(-damage);
                gameObject.SetActive(false);
            }
        }
    }
}
