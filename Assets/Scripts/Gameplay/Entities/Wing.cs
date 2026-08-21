using UnityEngine;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class Wing : MonoBehaviour, IDependencyInjectable
    {
        private const float MinDelay = 0.0005f;
        
        private PoolManager poolManager;
        private GameStateManager gameStateManager;
        public static float SkillCooltimeMax = 1f;
        public static float CriticalProb = 0f;
        public static bool Freezing = false;

        public void InjectDependency(GameDependencies dependencies)
        {
            poolManager = dependencies.PoolManager;
            gameStateManager = dependencies.GameStateManager;
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
            
            var fireball = poolManager.Spawn<WingBullet>();
            fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
            fireball.transform.position = transform.position;
        }
    }
}
