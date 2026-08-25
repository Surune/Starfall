using UnityEngine;
using Mirror;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class Wing : MonoBehaviour
    {
        private const float MinDelay = 0.0005f;
        
        private PoolManager poolManager;
        private GameStateManager gameStateManager;
        public static float SkillCooltimeMax = 1f;
        public static float CriticalProb = 0f;
        public static bool Freezing = false;

        public void Initialize(PoolManager poolManager, GameStateManager gameStateManager)
        {
            this.poolManager = poolManager;
            this.gameStateManager = gameStateManager;
        }

        private void Start()
        {
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void ChangeSkillCool(float newcooltime)
        {
            if (newcooltime <= MinDelay)
            {
                newcooltime = MinDelay;
            }
            
            SkillCooltimeMax = newcooltime;
            CancelInvoke(nameof(Shoot));
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void Shoot()
        {
            if (!gameStateManager.IsPlaying)
            {
                return;
            }
            
            if (NetworkClient.active || NetworkServer.active)
            {
                GameManager.Instance.Player.ShootWing(transform.position);
                return;
            }

            var fireball = poolManager.Spawn<WingBullet>();
            fireball.transform.rotation = Quaternion.identity;
            fireball.transform.position = transform.position;
        }
    }
}
