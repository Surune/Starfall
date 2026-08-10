using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity
{
    public class Wing : MonoBehaviour
    {
        private const float MinDelay = 0.0005f;
        
        private static PoolManager PoolManager => GameManager.Instance.PoolManager;
        public static float SkillCooltimeMax = 1f;
        public static float CriticalProb = 0f;
        public static bool Freezing = false;

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
            if (!GameStateManager.Instance.IsPlaying)
            {
                return;
            }
            
            var fireball = PoolManager.Get(PoolNumber.WingBullet);
            fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
            fireball.transform.position = transform.position;
        }
    }
}
