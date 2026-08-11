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
                var bulletLeft = PoolManager.Get(PoolNumber.Bullet);
                bulletLeft.transform.rotation = Quaternion.Euler(0, 0, -10);
                bulletLeft.transform.position = transform.position;
                PlayerManager.SetFireInfo(bulletLeft.GetComponent<Bullet>());

                var bulletRight = PoolManager.Get(PoolNumber.Bullet);
                bulletRight.transform.rotation = Quaternion.Euler(0, 0, 10);
                bulletRight.transform.position = transform.position;
                PlayerManager.SetFireInfo(bulletRight.GetComponent<Bullet>());
            }
            if (AbilityManager.fracture && !AbilityManager.awaken)
            {
                return;
            }

            var bullet = PoolManager.Get(PoolNumber.Bullet);
            PlayerManager.SetFireInfo(bullet.GetComponent<Bullet>());
            bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
            bullet.transform.position = transform.position;
        }

        public void Explode(Transform center, float coeff = 1f)
        {
            for (var i = -2; i <= 2; i++)
            {
                var bullet = PoolManager.Get(PoolNumber.Bullet);
                bullet.transform.rotation = Quaternion.Euler(0, 0, 45 * i);
                bullet.GetComponent<Bullet>().Damage = PlayerManager.damage * PlayerManager.damageCoefficient * coeff;
                PlayerManager.SetFireInfo(bullet.GetComponent<Bullet>());
                bullet.transform.position = center.position;
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
