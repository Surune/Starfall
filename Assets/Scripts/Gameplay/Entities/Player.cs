using UnityEngine;
using UnityEngine.InputSystem;
using Audio;
using Gameplay.Managers;
using Utilities;

namespace Gameplay.Entities
{
    public class Player : MonoBehaviour
    {
        private PlayerManager playerManager;
        private PoolManager poolManager;
        private SoundManager sound;
        private GameStateManager gameStateManager;

        public GameObject Barrier;
        public float SkillCooltimeMax = 0.2f;
        public int BulletCount = 1;
        [SerializeField] private float speed = 5f;
        [SerializeField] private InputActionReference move;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private Rigidbody2D rigidBody;
        [HideInInspector] public bool Reloading;
        [HideInInspector] public int KillNum = 0;

        private const float MinDelay = 0.0005f;
        private Vector2 moveDir;

        public void Initialize(PlayerManager playerManager, PoolManager poolManager, SoundManager sound, GameStateManager gameStateManager)
        {
            this.playerManager = playerManager;
            this.poolManager = poolManager;
            this.sound = sound;
            this.gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            InvokeRepeating(nameof(Shoot), 0f, SkillCooltimeMax);
        }

        private void Update()
        {
            moveDir = move.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            rigidBody.MovePosition(rigidBody.position + moveDir * (Time.fixedDeltaTime * speed));
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
            if (!gameStateManager.IsPlaying || Reloading)
            {
                return;
            }

            sound.PlaySFX(SoundKey.Shoot);
            for (var i = 0; i < BulletCount; i++)
            {
                var bullet = poolManager.Spawn<Bullet>();
                playerManager.SetFireInfo(bullet);
                bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
                bullet.transform.position = bulletSpawnPoint.position;
            }
        }

        public void Explode(Transform center, float coeff = 1f)
        {
            for (var i = -2; i <= 2; i++)
            {
                var bullet = poolManager.Spawn<Bullet>();
                bullet.transform.rotation = Quaternion.Euler(0, 0, 45 * i);
                bullet.Damage = playerManager.damage * playerManager.damageCoefficient * coeff;
                playerManager.SetFireInfo(bullet);
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
