using System.Collections.Generic;
using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;
using UnityEngine.InputSystem;

namespace Starfall.Entity
{
    public class Player : MonoBehaviour
    {
        static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
        static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
        static PoolManager PoolManager => GameManager.Instance.PoolManager;
        static GameObject EnemyList => GameManager.Instance.Spawner.EnemyList;
        static SFXManager Sfx => GameManager.Instance.SfxManager;

        public GameObject Barrier;
        public float SkillCooltimeMax;
        [SerializeField] private float speed = 3f;
        [SerializeField] private InputActionReference move;
        [HideInInspector] public bool Reloading;
        [HideInInspector] public int KillNum = 0;

        const float MinDelay = 0.0005f;

        private void Awake()
        {
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        private void Update()
        {
            var moveDir = (Vector3)move.action.ReadValue<Vector2>();
            transform.position += moveDir * (Time.deltaTime * speed);
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

        public void Magnetism(Transform center)
        {
            var minDist = 1.5f;
            foreach (var t in GameManager.GetAllChilds(EnemyList.transform))
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

            Sfx.PlayShoot();
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
            for (var i = -2; i <= 2; i++)
            {
                var fireball = PoolManager.Get(PoolNumber.Fireball);
                fireball.transform.rotation = Quaternion.Euler(0, 0, 45 * i);
                fireball.GetComponent<Fireball>().Damage = PlayerManager.damage * PlayerManager.damageCoefficient * coeff;
                PlayerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                fireball.transform.position = center.position;
            }
        }

        public void Echoshot(int shotnum)
        {
            for (var i = 0; i < shotnum; i++)
            {
                Invoke(nameof(Shoot), 0.1f * i);
            }
        }
    }
}
