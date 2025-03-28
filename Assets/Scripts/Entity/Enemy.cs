using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Starfall.Manager;
using Starfall.Constants;
using UnityEngine.Serialization;

namespace Starfall.Entity
{
    public class Enemy : MonoBehaviour
    {
        static ScoreManager ScoreManager => GameManager.Instance.ScoreManager;
        static ExpManager ExpManager => GameManager.Instance.ExpManager;
        static EffectManager EffectManager => GameManager.Instance.EffectManager;
        static SFXManager SfxManager => GameManager.Instance.SfxManager;
        static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
        static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
        static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;
        static PoolManager PoolManager => GameManager.Instance.PoolManager;
        static HPManager HpManager => GameManager.Instance.HPManager;
        static Player Player => GameManager.Instance.Player;

        [HideInInspector] public bool IsBoss = false;
        [HideInInspector] public float Maxspeed = 10f;
        [HideInInspector] public float SlowTime = 0f;
        [HideInInspector] public float MaxHP = 2f;
        public float CurrentHP = 2f;
        [HideInInspector] public int ExpAmount = 0;
        [HideInInspector] public bool MakeMeteor = false;
        public static float DamageCoefficient = 1f;    //damage coefficient
        public static float ItemProb = 3f;

        static readonly float[] BaseHPList = {0f, 1f, 1f, 1f, 1f, 1f, 1f};
        static readonly float[] StageHPList = {1f, 0.75f, 0.1f, 1f, 1.4f, 1.5f, 1.6f};
        [SerializeField] SpriteRenderer sprite;
        [SerializeField] EnemyType type;
        [SerializeField] TextMeshProUGUI resourceText;
        [SerializeField] Vector3 moveDirection = Vector3.down;
        float _speed;

        void Start()
        {
            enabled = GameStateManager.IsPlaying;
            SetHPText();
        }

        public void SetHPText()
        {
            resourceText.text = Mathf.CeilToInt(CurrentHP).ToString();
        }

        // type_num = 0 ~ 6
        public void SetType(int type_num)
        {
            type = (EnemyType)type_num;
            MaxHP = BaseHPList[type_num] + StageHPList[type_num] * (GameManager.Instance.Timer.WaveNum * GameManager.Instance.Timer.WaveNum + GameManager.Instance.Timer.RoundNum - 1);
            CurrentHP = MaxHP;
            _speed = Maxspeed;
            SetHPText();
            if (type == EnemyType.Blue)
            {
                var worldpos = Camera.main.WorldToViewportPoint(transform.position);
                if (worldpos.x < 0.5f)
                {
                    moveDirection = new Vector3(0.5f, -1, 0);
                }
                else
                {
                    moveDirection = new Vector3(-0.5f, -1, 0);
                }
            }
            else
            {
                moveDirection = Vector3.down;
            }
        }

        public void MakeBoss()
        {
            IsBoss = true;
            MaxHP += StageHPList[(int)type] * (GameManager.Instance.Timer.WaveNum * GameManager.Instance.Timer.WaveNum + 7);
            CurrentHP = MaxHP;
            Maxspeed *= 0.5f;
            _speed = Maxspeed;
            SetHPText();
            transform.localScale *= 2f;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false, bool fatal = false)
        {
            dmg *= DamageCoefficient;
            if (AbilityManager.culling && Mathf.Approximately(MaxHP, CurrentHP))
            {
                dmg *= 1.25f;
            }
            dmg = dmg < 0f ? 0f : dmg;
            CurrentHP -= dmg;
            EffectManager.SetDamageEffect(transform.position, dmg, isCritical : critical, isFatal : fatal);
            if (critical)
            {
                if (AbilityManager.fifth)
                {
                    PlayerManager.criticalCount++;
                    if(PlayerManager.criticalCount % 5 == 0)
                    {
                        Player.Echoshot(1);
                    }
                }
                if (AbilityManager.psychosense)
                {
                    var fireball = PoolManager.Get(PoolNumber.Fireball);
                    fireball.transform.position = Player.transform.position;
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.GetComponent<Fireball>().Damage = 3f;
                }
            }

            var p = PoolManager.Get(PoolNumber.Effect);
            p.transform.position = transform.position;

            SetHPText();
            bool dead = CheckIfDead(fatal);
            if (!mute && !dead)
            {
                if (critical)
                {
                    SfxManager.PlayEnemySound(isCritical : true);
                }
                else
                {
                    SfxManager.PlayEnemySound();
                }
            }
            if (dead && AbilityManager.noxious)
            {
                PlayerManager.DamageAllEnemy(PlayerManager.damage * PlayerManager.damageCoefficient + PlayerManager.fixDamage);
            }
            return dead;
        }

        public bool GetHeal(float heal_amount)
        {
            if (MaxHP - CurrentHP > 0.0001f)
            {
                heal_amount = CurrentHP + heal_amount > MaxHP ? MaxHP - CurrentHP : heal_amount;
                CurrentHP += heal_amount;
                EffectManager.SetDamageEffect(transform.position, heal_amount, isCritical : false, isFatal : false, isHeal : true);
                SetHPText();
                return true;
            }
            else
            {
                return false;
            }
        }

        bool CheckIfDead(bool fatal = false)
        {
            bool isDead = false;
            if (fatal == true && IsBoss == false && gameObject.activeSelf)
            {
                if (AbilityManager.kineticBarrage)
                {
                    Player.Echoshot(2);
                }
                isDead = true;
            }
            else if (CurrentHP <= 0f && gameObject.activeSelf)
            {
                if (type == EnemyType.Green)
                {
                    List<Transform> enemies = GameManager.GetAllChilds(GameManager.Instance.EnemyList);
                    foreach (Transform e in enemies)
                    {
                        if (e == transform)
                        {
                            continue;
                        }
                        e.GetComponent<Enemy>().GetHeal(GameManager.Instance.Timer.WaveNum);
                    }
                }
                else if (type == EnemyType.Violet)
                {
                    var fireballs = GameManager.GetAllChilds(GameManager.Instance.FireballList);
                    foreach (Transform f in fireballs)
                    {
                        f.gameObject.SetActive(false);
                    }
                }
                isDead = true;
            }

            if (isDead)
            {
                GameManager.Instance.ActiveEnemyNum--;
                Player.KillNum++;

                if (type == EnemyType.Indigo || MakeMeteor)
                {
                    GameManager.Instance.Spawner.SpawnMeteor();
                }

                var effect = PoolManager.Get(PoolNumber.DamageEffect);
                effect.transform.position = transform.position;
                effect.transform.localScale = transform.localScale;

                var p = EffectManager.GetComponent<ParticleSystem>().main;
                p.startColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;

                EffectManager.Exp.ExpAmount = ExpAmount;
                if (fatal == true && AbilityManager.firm)
                {
                    EffectManager.Exp.ExpAmount += 1;
                }
                if (AbilityManager.burning && type == EnemyType.Red)
                {
                    EffectManager.Exp.ExpAmount += 1;
                }

                if (AbilityManager.echo)
                {
                    Player.Echoshot(2);
                }
                if (AbilityManager.magnet)
                {
                    Player.Magnetism(transform);
                }
                if (AbilityManager.explode)
                {
                    Player.Explode(transform, 1f);
                }

                //effect.PlayEnemySound(isKilled : true);
                ScoreManager.GetScore(ExpAmount);
                if (Random.Range(0, 100) < ItemProb)
                {
                    GameObject item = PoolManager.Get(PoolNumber.Item);
                    item.transform.position = transform.position;
                    item.GetComponent<DropItem>().SetType((ItemType)Random.Range(0, 4));
                }
                gameObject.SetActive(false);
            }

            return isDead;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Area"))
            {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.Slow)
                {
                    _speed = Maxspeed * 0.75f;
                }
                if (collidedarea.Damage)
                {
                    CurrentHP -= Time.deltaTime;
                }
            }
            else if (collision.transform.CompareTag("Player"))
            {
                HpManager.GetDamage(-Mathf.CeilToInt(CurrentHP));
                GameManager.Instance.ActiveEnemyNum -= 1;
                ExpManager.GetExp(ExpAmount);
                ScoreManager.GetScore(ExpAmount);
                gameObject.SetActive(false);
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Area"))
            {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.Slow)
                {
                    _speed = Maxspeed * 0.75f;
                }
                if (collidedarea.Damage)
                {
                    CurrentHP -= Time.deltaTime;
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Area"))
            {
                _speed = Maxspeed;
            }
        }

        void CheckInvisible()
        {
            Vector3 worldpos = Camera.main.WorldToViewportPoint(transform.position);
            if (worldpos.y < 0f)
            {
                gameObject.SetActive(false);
                GameManager.Instance.ActiveEnemyNum -= 1;
                GameManager.Instance.Spawner.SpawnMeteor();
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

        void Update()
        {
            if (GameStateManager.Instance.IsPlaying)
            {
                if (SlowTime > 0f)
                {
                    SlowTime -= Time.deltaTime;
                    _speed = Maxspeed * 0.75f;
                    if (SlowTime <= 0f)
                    {
                        SlowTime = 0f;
                        _speed = Maxspeed;
                    }
                }
                transform.Translate(moveDirection * _speed * Time.deltaTime);
                CheckInvisible();
            }
        }
    }
}
