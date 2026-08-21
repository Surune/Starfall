using System;
using UnityEngine;
using Audio;
using Core.Constants;
using Data.Enemies;
using Gameplay.Effects;
using Gameplay.Managers;
using Gameplay.Spawning;
using Random = UnityEngine.Random;

namespace Gameplay.Entities
{
    public class Enemy : MonoBehaviour, IDependencyInjectable, IPoolable, IDamageable
    {
        private ExpManager expManager;
        private EffectManager effectManager;
        private GameStateManager gameStateManager;
        private PoolManager poolManager;
        private HPManager hpManager;
        private SoundManager sound;
        private Player player;
        private Timer timer;
        private Spawner spawner;
        private System.Action enemyRemoved;

        public bool IsBoss { get; set; }
        [HideInInspector] public float Maxspeed = 10f;
        [HideInInspector] public float SlowTime = 0f;
        [HideInInspector] public float MaxHP = 2f;
        public float CurrentHP = 2f;
        [HideInInspector] public int ExpAmount = 0;
        [HideInInspector] public bool MakeMeteor = false;
        public static float DamageCoefficient = 1f;
        public static float ItemProb = 3f;

        [SerializeField] private EnemyType type;
        [SerializeField] private SpriteAnimation spriteAnimation;
        [SerializeField] private Vector3 moveDirection = Vector3.down;
        private EnemyData enemyData;
        private float speed;

        public void InjectDependency(GameDependencies dependencies)
        {
            expManager = dependencies.ExpManager;
            effectManager = dependencies.EffectManager;
            gameStateManager = dependencies.GameStateManager;
            poolManager = dependencies.PoolManager;
            hpManager = dependencies.HPManager;
            sound = dependencies.SoundManager;
            player = dependencies.Player;
            timer = dependencies.Timer;
            spawner = dependencies.Spawner;
            enemyRemoved = dependencies.EnemyRemoved;
        }

        public void OnSpawn()
        {
            IsBoss = false;
            SlowTime = 0f;
            MakeMeteor = false;
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
            MaxHP = enemyData.BaseHP + enemyData.StageHP * (timer.WaveNum * timer.WaveNum + timer.RoundNum - 1);
            CurrentHP = MaxHP;
            speed = Maxspeed;
            if (type == EnemyType.Blue)
            {
                var worldpos = Camera.main.WorldToViewportPoint(transform.position);
                moveDirection = worldpos.x < 0.5f ? new Vector3(0.5f, -1, 0) : new Vector3(-0.5f, -1, 0);
            }
            else
            {
                moveDirection = Vector3.down;
            }
        }

        public void MakeBoss()
        {
            IsBoss = true;
            MaxHP += enemyData.StageHP * (timer.WaveNum * timer.WaveNum + 7);
            CurrentHP = MaxHP;
            Maxspeed *= 0.5f;
            speed = Maxspeed;
            transform.localScale *= 2f;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false, bool fatal = false)
        {
            dmg *= DamageCoefficient;
            dmg = dmg < 0f ? 0f : dmg;
            CurrentHP -= dmg;
            effectManager.SetDamageEffect(transform.position, dmg, isCritical : critical, isFatal : fatal);
            if (critical)
            {
            }
            
            var dead = CheckIfDead(fatal);
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
            SlowTime = duration;
        }

        private bool GetHeal(float healAmount)
        {
            if (MaxHP - CurrentHP > 0.0001f)
            {
                healAmount = CurrentHP + healAmount > MaxHP ? MaxHP - CurrentHP : healAmount;
                CurrentHP += healAmount;
                effectManager.SetDamageEffect(transform.position, healAmount, isCritical : false, isFatal : false, isHeal : true);
                return true;
            }
            
            return false;
        }

        private bool CheckIfDead(bool fatal = false)
        {
            var isDead = false;
            if (fatal && IsBoss == false && gameObject.activeSelf)
            {
                isDead = true;
            }
            else if (CurrentHP <= 0f && gameObject.activeSelf)
            {
                if (type == EnemyType.Green)
                {
                    foreach (var enemy in spawner.ActiveEnemies)
                    {
                        if (enemy == this)
                        {
                            continue;
                        }
                        enemy.GetHeal(timer.WaveNum);
                    }
                }
                else if (type == EnemyType.Violet)
                {
                    throw new NotImplementedException();
                }
                isDead = true;
            }

            if (!isDead)
            {
                return false;
            }
            
            player.KillNum++;

            if (type == EnemyType.Indigo || MakeMeteor)
            {
                spawner.SpawnMeteor();
            }

            var effect = poolManager.Spawn<DamageEffect>();
            effect.transform.position = transform.position;
            effect.transform.localScale = transform.localScale;

            //effect.PlayEnemySound(isKilled : true);
            expManager.GetExp(ExpAmount);
            if (Random.Range(0, 100) < ItemProb)
            {
                var item = poolManager.Spawn<DropItem>();
                item.transform.position = transform.position;
                item.SetType((ItemType)Random.Range(0, 4));
            }
            DespawnFromGame();

            return isDead;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                hpManager.GetDamage(-Mathf.CeilToInt(CurrentHP));
                expManager.GetExp(ExpAmount);
                DespawnFromGame();
            }
        }

        private void DespawnFromGame()
        {
            spawner.RemoveActiveEnemy(this);
            enemyRemoved();
            gameObject.SetActive(false);
        }

        private void CheckInvisible()
        {
            var worldpos = Camera.main.WorldToViewportPoint(transform.position);
            if (worldpos.y < 0f)
            {
                DespawnFromGame();
                spawner.SpawnMeteor();
            }
            else if (worldpos.x < 0f)
            {
                moveDirection = new Vector3(0.5f, -1, 0);
            }
            else if (worldpos.x > 1f)
            {
                moveDirection = new Vector3(-0.5f, -1, 0);
            }
        }

        private void Update()
        {
            if (!gameStateManager.IsPlaying)
            {
                return;
            }
            
            if (SlowTime > 0f)
            {
                SlowTime -= Time.deltaTime;
                speed = Maxspeed * 0.75f;
                if (SlowTime <= 0f)
                {
                    SlowTime = 0f;
                    speed = Maxspeed;
                }
            }
            transform.Translate(moveDirection * speed * Time.deltaTime);
            CheckInvisible();
        }
    }
}
