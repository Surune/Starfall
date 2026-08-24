using UnityEngine;
using Audio;
using Core.Constants;
using Data.Enemies;
using Gameplay.Managers;

namespace Gameplay.Entities
{
    public class Enemy : MonoBehaviour, IPoolable, IDamageable
    {
        private EffectManager effectManager;
        private GameStateManager gameStateManager;
        private HPManager hpManager;
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

        public void Initialize(EffectManager effectManager, GameStateManager gameStateManager, HPManager hpManager, SoundManager sound, Timer timer, EnemyDeathResolver deathResolver)
        {
            this.effectManager = effectManager;
            this.gameStateManager = gameStateManager;
            this.hpManager = hpManager;
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

        private void Start()
        {
            enabled = gameStateManager.IsPlaying;
        }

        public void SetType(EnemyData data)
        {
            enemyData = data;
            type = enemyData.Type;
            spriteAnimation.SetSprites(enemyData.Sprites);
            health.SetMaximum(enemyData.BaseHP + enemyData.StageHP * (timer.WaveNum * timer.WaveNum + timer.RoundNum - 1));
            movement.Configure(type, Camera.main, transform.position);
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
            effectManager.SetDamageEffect(transform.position, dmg, isCritical : critical, isFatal : fatal);
            
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

        public void ApplySlow(float duration)
        {
            movement.ApplySlow(duration);
        }

        internal bool Heal(float healAmount)
        {
            var healedAmount = health.Heal(healAmount);
            if (healedAmount > 0f)
            {
                effectManager.SetDamageEffect(transform.position, healedAmount, isCritical : false, isFatal : false, isHeal : true);
                return true;
            }

            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                hpManager.GetDamage(-Mathf.CeilToInt(health.Current));
                deathResolver.Despawn(this);
            }
        }

        private void Update()
        {
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
