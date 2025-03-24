using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity
{
    public class Wing : MonoBehaviour
    {
        static PoolManager PoolManager => GameManager.Instance.PoolManager;

        readonly float minDelay = 0.0005f;
        public static float SkillCooltimeMax = 1f;
        public static float CriticalProb = 0f;
        public static bool Freezing = false;

        void Start()
        {
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void ChangeSkillCool(float newcooltime)
        {
            if (newcooltime <= minDelay)
            {
                newcooltime = minDelay;
            }
            SkillCooltimeMax = newcooltime;
            CancelInvoke(nameof(Shoot));
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void Shoot()
        {
            if (GameStateManager.Instance.IsPlaying)
            {
                var fireball = PoolManager.Get(PoolNumber.WingBullet);
                fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                fireball.transform.position = transform.position;
            }
        }
    }
}
