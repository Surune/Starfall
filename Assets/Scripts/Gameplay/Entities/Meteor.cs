using UnityEngine;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class Meteor : MonoBehaviour, IDependencyInjectable, IPoolable
    {
        private HPManager hpManager;
        private Timer timer;
        private GameStateManager gameStateManager;
        
        public float speed = 10f;
        private Vector3 moveDirection = Vector3.down;
        private int damage;

        public void InjectDependency(GameDependencies dependencies)
        {
            hpManager = dependencies.HPManager;
            timer = dependencies.Timer;
            gameStateManager = dependencies.GameStateManager;
            damage = timer.WaveNum;
        }

        public void OnSpawn()
        {
            speed = 10f;
            moveDirection = Vector3.down;
            damage = timer.WaveNum;
        }

        public void OnDespawn()
        {
        }

        private void Update()
        {
            if (gameObject.activeSelf && gameStateManager.IsPlaying)
            {
                transform.position += moveDirection * (speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.transform.CompareTag("Player"))
            {
                return;
            }
            
            hpManager.GetDamage(-damage);
            gameObject.SetActive(false);
        }
    }
}
