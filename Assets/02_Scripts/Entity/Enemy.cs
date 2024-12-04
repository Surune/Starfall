using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Starfall.Manager;
using Starfall.Constants;

namespace Starfall.Entity {
    public enum EnemyType {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet
    }

    public class Enemy : MonoBehaviour {
        # region Managers
        [SerializeField]    private SpriteRenderer sprite;
                            private static ScoreManager scoreManager => GameManager.Instance.ScoreManager;
                            private static ExpManager expManager => GameManager.Instance.exp;
                            private static EffectManager effectManager => GameManager.Instance.EffectManager;
                            private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
                            private static AbilityManager abilityManager => GameManager.Instance.AbilityManager;
                            private static GameStateManager gameStateManager => GameManager.Instance.gameStateManager;
                            private static PoolManager poolManager => GameManager.Instance.PoolManager;
                            private static HPManager hpManager => GameManager.Instance.HPManager;
                            private static Player player => GameManager.Instance.Player;
        # endregion
        
        # region Property
        [SerializeField]    private EnemyType type;
        [SerializeField]    private TextMeshProUGUI resourceText;
        [HideInInspector]   public bool isBoss = false;
                            private static float[] baseHPList = {0f, 1f, 1f, 1f, 1f, 1f, 1f};
                            private static float[] stageHPList = {1f, 0.75f, 0.1f, 1f, 1.4f, 1.5f, 1.6f};
        [HideInInspector]   public float maxspeed = 10f;
                            private float speed;
        [HideInInspector]   public float slowTime = 0f;
        [HideInInspector]   public float maxHP = 2f;
                            public float currentHP = 2f;
                            public static float damageCoefficient = 1f;    //damage coefficient
        [SerializeField]    private Vector3 moveDirection = new Vector3(0, -1, 0);
        [HideInInspector]   public int expAmount = 0;
        [HideInInspector]   public bool makeMeteor = false;
                            public static float itemProb = 3f;
        # endregion

        void Start () {
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
            enabled = gameStateManager.IsPlaying;
            SetHPText();
        }
        
        public void SetHPText(){
            resourceText.text = "" + Mathf.CeilToInt(currentHP);
        }

        void OnDestroy() {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        // type_num = 0 ~ 6
        public void SetType(int type_num) {
            type = (EnemyType)type_num;
            maxHP = baseHPList[type_num] + stageHPList[type_num] * (GameManager.Instance.timer.waveNum * GameManager.Instance.timer.waveNum + GameManager.Instance.timer.roundNum - 1);
            currentHP = maxHP;
            speed = maxspeed;
            SetHPText();
            if (type == EnemyType.Blue) {
                var worldpos = Camera.main.WorldToViewportPoint(this.transform.position);
                if (worldpos.x < 0.5f)
                    moveDirection = new Vector3(0.5f, -1, 0);
                else
                    moveDirection = new Vector3(-0.5f, -1, 0);
            }
            else    
                moveDirection = Vector3.down;
        }

        public void MakeBoss() {
            isBoss = true;
            maxHP += stageHPList[(int)type] * (GameManager.Instance.timer.waveNum * GameManager.Instance.timer.waveNum + 7);
            currentHP = maxHP;
            maxspeed *= 0.5f;
            speed = maxspeed;
            SetHPText();
            this.transform.localScale *= 2f;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false, bool fatal = false) {
            dmg *= damageCoefficient;
            if (abilityManager.culling && maxHP == currentHP) {
                dmg *= 1.25f;
            }
            dmg = dmg < 0f ? 0f : dmg;
            currentHP -= dmg;
            effectManager.SetDamageEffect(transform.position, dmg, isCritical : critical, isFatal : fatal);
            if (critical) {
                if (abilityManager.fifth) {
                    playerManager.criticalCount++;
                    if(playerManager.criticalCount % 5 == 0) {
                        player.Echoshot(1);
                    }
                }
                if (abilityManager.psychosense) {
                    var fireball = poolManager.Get(PoolNumber.Fireball);
                    fireball.transform.position = player.transform.position;
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.GetComponent<Fireball>().damage = 3f;
                }
            }
            
            var p = poolManager.Get(PoolNumber.Effect);
            p.transform.position = transform.position;
            
            SetHPText();
            bool dead = CheckIfDead(fatal);
            if (!mute && !dead) {
                if (critical)   effectManager.PlayEnemySound(isCritical : true);
                else    effectManager.PlayEnemySound();
            }
            if (dead && abilityManager.noxious) {
                playerManager.DamageAllEnemy(playerManager.damage * playerManager.damageCoefficient + playerManager.fixDamage);
            }
            return dead;
        }

        public bool GetHeal(float heal_amount) {
            if (maxHP - currentHP > 0.0001f) {
                heal_amount = currentHP + heal_amount > maxHP ? maxHP - currentHP : heal_amount;
                currentHP += heal_amount;
                effectManager.SetDamageEffect(transform.position, heal_amount, isCritical : false, isFatal : false, isHeal : true);
                SetHPText();
                return true;
            }
            else
                return false;
        }

        private bool CheckIfDead(bool fatal = false) {
            bool isDead = false;
            if (fatal == true && isBoss == false && gameObject.activeSelf) {
                if (abilityManager.kineticBarrage)  player.Echoshot(2);
                isDead = true;
            }
            else if (currentHP <= 0f && gameObject.activeSelf) {
                if (type == EnemyType.Green) {
                    List<Transform> enemies = GameManager.GetAllChilds(GameManager.Instance.enemyList);
                    foreach(Transform e in enemies) {
                        if (e == this.transform) {
                            continue;
                        }
                        e.GetComponent<Enemy>().GetHeal(GameManager.Instance.timer.waveNum);
                    }
                }
                else if (type == EnemyType.Violet) {
                    var fireballs = GameManager.GetAllChilds(GameManager.Instance.fireballList);
                    foreach(Transform f in fireballs){
                        f.gameObject.SetActive(false);
                    }
                }
                isDead = true;
            }

            if (isDead) {
                GameManager.Instance.activeEnemyNum--;
                player.killNum++;

                if (type == EnemyType.Indigo || makeMeteor)
                    GameManager.Instance.spawner.SpawnMeteor();

                var effect = poolManager.Get(PoolNumber.DamageEffect);
                effect.transform.position = transform.position;
                effect.transform.localScale = transform.localScale;
                var p = effectManager.GetComponent<ParticleSystem>().main;
                p.startColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;

                effectManager.GetComponent<Exp>().exp_amount = expAmount;
                if (fatal == true && abilityManager.firm)    effectManager.GetComponent<Exp>().exp_amount += 1;
                if (abilityManager.burning && type == EnemyType.Red)   effectManager.GetComponent<Exp>().exp_amount += 1;

                if (abilityManager.echo)     player.Echoshot(2);
                if (abilityManager.magnet)   player.Magnetism(this.transform);
                if (abilityManager.explode)  player.Explode(this.transform, 1f);

                //effect.PlayEnemySound(isKilled : true);
                scoreManager.GetScore(expAmount);
                if(Random.Range(0, 100) < itemProb) {
                    GameObject item = poolManager.Get(PoolNumber.Item);
                    item.transform.position = transform.position;
                    item.GetComponent<DropItem>().SetType(Random.Range(0, 4));
                }
                gameObject.SetActive(false);
            }

            return isDead;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.transform.tag == "Area") {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.slow) speed = maxspeed * 0.75f;
                if (collidedarea.damage) currentHP -= Time.deltaTime;
            }
            else if (collision.transform.tag == "Player") {
                hpManager.GetDamage(-Mathf.CeilToInt(currentHP));
                GameManager.Instance.activeEnemyNum -= 1;
                expManager.GetExp(expAmount);
                scoreManager.GetScore(expAmount);
                gameObject.SetActive(false);
            }
        }
        
        private void OnTriggerStay2D(Collider2D collision) {
            if (collision.transform.tag == "Area") {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.slow) speed = maxspeed * 0.75f;
                if (collidedarea.damage) currentHP -= Time.deltaTime;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.transform.tag == "Area")
                speed = maxspeed;
        }

        private void CheckInvisible() {
            Vector3 worldpos = Camera.main.WorldToViewportPoint(transform.position);
            if (worldpos.y < 0f) {
                gameObject.SetActive(false);
                GameManager.Instance.activeEnemyNum -= 1;
                GameManager.Instance.spawner.SpawnMeteor();
            }
            else if (worldpos.x < 0f)
                moveDirection = new Vector3(0.5f, -1, 0);
            else if (worldpos.x > 1f)
                moveDirection = new Vector3(-0.5f, -1, 0);
        }

        void Update () {
            if (enabled) {
                if (slowTime > 0f) {
                    slowTime -= Time.deltaTime;
                    speed = maxspeed * 0.75f;
                    if (slowTime <= 0f) {
                        slowTime = 0f;
                        speed = maxspeed;
                    }
                }
                transform.Translate(moveDirection * speed * Time.deltaTime);
                CheckInvisible();
            }
        }

        private void OnGameStateChanged(GameState newGameState) {
            enabled = (newGameState == GameState.Gameplay);
        }
    }
}