using Mirror;
using UnityEngine;
using Audio;
using Core.Constants;
using Data.Enemies;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class Enemy : NetworkBehaviour, IPoolable, IDamageable
    {
        private GameStateManager gameStateManager;
        private SoundManager sound;
        private Timer timer;
        private EnemyDeathResolver deathResolver;
        private readonly EnemyMovement movement = new();
        private readonly EnemyHealth health = new();

        public bool IsBoss { get; set; }
        public float Maxspeed
        {
            get => movement.MaxSpeed;
            set => movement.MaxSpeed = value;
        }
        public float CurrentHealth => health.Current;
        public static float DamageCoefficient = 1f;
        public static float ItemProb = 3f;

        [SerializeField] private EnemyType type;
        [SerializeField] private SpriteAnimation spriteAnimation;
        private EnemyData enemyData;

        [SyncVar(hook = nameof(OnEnemyDataIndexChanged))]
        private int enemyDataIndex = -1;

        private void Awake()
        {
            spriteAnimation.enabled = false;
        }

        public void Initialize(GameStateManager gameStateManager, SoundManager sound, Timer timer, EnemyDeathResolver deathResolver)
        {
            this.gameStateManager = gameStateManager;
            this.sound = sound;
            this.timer = timer;
            this.deathResolver = deathResolver;
        }

        public void OnSpawn()
        {
            IsBoss = false;
            movement.Reset();
            transform.localScale = Vector3.one;
        }

        public void OnDespawn()
        {
        }

        public override void OnStartClient()
        {
            transform.SetParent(GameManager.Instance.PoolManager.EntitiesTransform);
        }

        private void Start()
        {
            if (!isServer)
            {
                enabled = false;
                return;
            }

            enabled = gameStateManager.IsPlaying;
        }

        public void SetType(EnemyData data, int dataIndex)
        {
            enemyDataIndex = dataIndex;
            enemyData = data;
            type = enemyData.Type;
            spriteAnimation.SetSprites(enemyData.Sprites);
            spriteAnimation.enabled = true;
            health.SetMaximum(enemyData.BaseHP + enemyData.StageHP * (timer.WaveNum * timer.WaveNum + timer.RoundNum - 1));
            movement.Configure(type, Camera.main, transform.position);
        }

        private void OnEnemyDataIndexChanged(int _, int value)
        {
            var data = GameManager.Instance.Spawner.GetEnemyData(value);
            type = data.Type;
            spriteAnimation.SetSprites(data.Sprites);
            spriteAnimation.enabled = true;
        }

        public void MakeBoss()
        {
            IsBoss = true;
            health.IncreaseMaximum(enemyData.StageHP * (timer.WaveNum * timer.WaveNum + 7));
            movement.MakeBoss();
            transform.localScale *= 2f;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false, bool fatal = false)
        {
            dmg = health.TakeDamage(dmg * DamageCoefficient);
            RpcShowDamageEffect(transform.position, dmg, critical, fatal, false);
            
            var dead = deathResolver.TryResolve(this, type, fatal, ItemProb);
            if (!mute && !dead)
            {
                sound.PlaySFX(critical ? SoundKey.EnemyCritical : SoundKey.EnemyHit);
            }
            
            return dead;
        }

        public void ApplyDamage(float damage, bool critical = false, bool mute = false, bool fatal = false)
        {
            GetDamage(damage, critical, mute, fatal);
        }

        [ClientRpc]
        private void RpcShowDamageEffect(Vector3 position, float damage, bool critical, bool fatal, bool heal)
        {
            GameManager.Instance.EffectManager.SetDamageEffect(position, damage, isCritical : critical, isFatal : fatal, isHeal : heal);
        }

        public void ApplySlow(float duration)
        {
            movement.ApplySlow(duration);
        }

        internal bool Heal(float healAmount)
        {
            var healedAmount = health.Heal(healAmount);
            if (healedAmount > 0f)
            {
                RpcShowDamageEffect(transform.position, healedAmount, false, false, true);
                return true;
            }

            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isServer)
            {
                return;
            }

            if (collision.transform.CompareTag("Player"))
            {
                collision.GetComponent<PlayerHPManager>().GetDamage(Mathf.CeilToInt(health.Current));
                deathResolver.Despawn(this);
            }
        }

        private void Update()
        {
            if (!isServer)
            {
                return;
            }

            if (!gameStateManager.IsPlaying)
            {
                return;
            }
            
            if (!movement.Move(transform, Camera.main, Time.deltaTime))
            {
                deathResolver.Despawn(this);
            }
        }
    }
}
