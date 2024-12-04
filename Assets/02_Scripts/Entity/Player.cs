using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity {
    public class Player : MonoBehaviour {
        #region Manager
        private static AbilityManager abilityManager => GameManager.Instance.AbilityManager;
        private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
        private static PoolManager poolManager => GameManager.Instance.PoolManager;
        #endregion

        public float skillCooltimeMax;
        public float speed = 1f;
        [HideInInspector] public bool reloading;
        public GameObject barrier;
        private Transform EnemyList;
        private float mindelay = 0.0005f;
        private AudioSource musicPlayer;
        [SerializeField] private AudioClip sfxPlayerHit;
        [SerializeField] private AudioClip sfxShoot;
        
        [HideInInspector] public int killNum = 0;

        void Start () {
            EnemyList = GameObject.Find("Enemies").transform;
            musicPlayer = gameObject.GetComponent<AudioSource>();
            InvokeRepeating("Shoot", 0f, skillCooltimeMax);
        }

        public void ChangeSkillCool(float newcooltime) {
            if (newcooltime <= mindelay) newcooltime = mindelay;
            skillCooltimeMax = newcooltime;
            CancelInvoke("Shoot");
            InvokeRepeating("Shoot", 0f, skillCooltimeMax);
        }

        public void Magnetism(Transform center) {
            float minDist = 1.5f;
            foreach (Transform t in GameManager.GetAllChilds(EnemyList)) {
                if (t == center)
                    continue;
                
                if (Vector3.Distance(t.position, center.position) <= minDist)
                    t.position = Vector3.Lerp(t.position, center.position, 0.5f);
            }
        }

        public void Shoot() {
            if (!GameStateManager.Instance.IsPlaying || reloading)
                return;

            musicPlayer.PlayOneShot(sfxShoot);
            if (abilityManager.awaken || abilityManager.fracture) {
                var fireball_l = poolManager.Get(PoolNumber.Fireball);
                fireball_l.transform.rotation = Quaternion.Euler(0, 0, -10);
                fireball_l.transform.position = this.transform.position;
                playerManager.SetFireInfo(fireball_l.GetComponent<Fireball>());

                var fireball_r = poolManager.Get(PoolNumber.Fireball);
                fireball_r.transform.rotation = Quaternion.Euler(0, 0, 10);
                fireball_r.transform.position = this.transform.position;
                playerManager.SetFireInfo(fireball_r.GetComponent<Fireball>());
            }
            if (abilityManager.fracture && !abilityManager.awaken) return;

            var fireball = poolManager.Get(PoolNumber.Fireball);
            fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
            playerManager.SetFireInfo(fireball.GetComponent<Fireball>());
            fireball.transform.position = this.transform.position;
        }

        public void Explode(Transform center, float coeff = 1f) {
            for (int i = -2; i <= 2; i++) {
                var fireball = poolManager.Get(PoolNumber.Fireball);
                fireball.transform.rotation = Quaternion.Euler(0, 0, 360/8*i);
                fireball.GetComponent<Fireball>().damage = playerManager.damage * playerManager.damageCoefficient * coeff;
                playerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                fireball.transform.position = center.position;
            }
        }

        public void Echoshot(int shotnum) {
            for (int i = 0; i < shotnum; i++)
                Invoke("Shoot", 0.1f * i);
        }
    }
}