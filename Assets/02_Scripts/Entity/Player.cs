using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;
using UnityEngine.Serialization;

namespace Starfall.Entity
{
    public class Player : MonoBehaviour
    {
        static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
        static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
        static PoolManager PoolManager => GameManager.Instance.PoolManager;
        static GameObject EnemyList => GameManager.Instance.Spawner.EnemyList;

        public GameObject Barrier;
        public AudioSource MusicPlayer;
        public float SkillCooltimeMax;
        public float Speed = 1f;
        [HideInInspector] public bool Reloading;

        readonly float minDelay = 0.0005f;
        [SerializeField] AudioClip sfxPlayerHit;
        [SerializeField] AudioClip sfxShoot;

        [HideInInspector] public int KillNum = 0;

        void Awake()
        {
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void ChangeSkillCool(float newcooltime)
        {
            if (newcooltime <= minDelay) newcooltime = minDelay;
            SkillCooltimeMax = newcooltime;
            CancelInvoke(nameof(Shoot));
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        public void Magnetism(Transform center)
        {
            float minDist = 1.5f;
            foreach (Transform t in GameManager.GetAllChilds(EnemyList.transform))
            {
                if (t == center)
                {
                    continue;
                }

                if (Vector3.Distance(t.position, center.position) <= minDist)
                {
                    t.position = Vector3.Lerp(t.position, center.position, 0.5f);
                }
            }
        }

        public void Shoot()
        {
            if (!GameStateManager.Instance.IsPlaying || Reloading)
            {
                return;
            }

            MusicPlayer.PlayOneShot(sfxShoot);
            if (AbilityManager.awaken || AbilityManager.fracture)
            {
                var fireball_l = PoolManager.Get(PoolNumber.Fireball);
                fireball_l.transform.rotation = Quaternion.Euler(0, 0, -10);
                fireball_l.transform.position = transform.position;
                PlayerManager.SetFireInfo(fireball_l.GetComponent<Fireball>());

                var fireball_r = PoolManager.Get(PoolNumber.Fireball);
                fireball_r.transform.rotation = Quaternion.Euler(0, 0, 10);
                fireball_r.transform.position = transform.position;
                PlayerManager.SetFireInfo(fireball_r.GetComponent<Fireball>());
            }
            if (AbilityManager.fracture && !AbilityManager.awaken)
            {
                return;
            }

            var fireball = PoolManager.Get(PoolNumber.Fireball);
            PlayerManager.SetFireInfo(fireball.GetComponent<Fireball>());
            fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
            fireball.transform.position = transform.position;
        }

        public void Explode(Transform center, float coeff = 1f)
        {
            for (int i = -2; i <= 2; i++)
            {
                var fireball = PoolManager.Get(PoolNumber.Fireball);
                fireball.transform.rotation = Quaternion.Euler(0, 0, 360/8*i);
                fireball.GetComponent<Fireball>().Damage = PlayerManager.damage * PlayerManager.damageCoefficient * coeff;
                PlayerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                fireball.transform.position = center.position;
            }
        }

        public void Echoshot(int shotnum)
        {
            for (int i = 0; i < shotnum; i++)
            {
                Invoke(nameof(Shoot), 0.1f * i);
            }
        }
    }
}
