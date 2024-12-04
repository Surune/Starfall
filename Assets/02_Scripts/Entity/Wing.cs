using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity {
    public class Wing : MonoBehaviour {
        #region Manager
        private static PoolManager poolManager => GameManager.Instance.PoolManager;
        #endregion
        public static float skillCooltimeMax = 1f;
        public static float criticalProb = 0f;
        private float minDelay = 0.0005f;

        public static bool freezing = false;

        private void Start() {
            InvokeRepeating("Shoot", 0f, skillCooltimeMax);
        }

        public void ChangeSkillCool(float newcooltime) {
            if (newcooltime <= minDelay) newcooltime = minDelay;
            skillCooltimeMax = newcooltime;
            CancelInvoke("Shoot");
            InvokeRepeating("Shoot", 0f, skillCooltimeMax);
        }

        public void Shoot() {
            if (GameStateManager.Instance.IsPlaying) {
                var fireball = poolManager.Get(PoolNumber.WingBullet);
                fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                fireball.transform.position = this.transform.position;
            }
        }
    }
}
