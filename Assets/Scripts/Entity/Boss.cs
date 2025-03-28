using UnityEngine;
using TMPro;
using Starfall.Manager;
using Starfall.Constants;
using UnityEngine.Serialization;

namespace Starfall.Entity
{
    public class Boss : MonoBehaviour
    {
        static EffectManager EffectManager => GameManager.Instance.EffectManager;
        static SFXManager SfxManager => GameManager.Instance.SfxManager;
        static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
        static PoolManager PoolManager => GameManager.Instance.PoolManager;
        static Player Player => GameManager.Instance.Player;
        public GameObject GameClearDisplay;
        public bool IsBoss = true;
        public float Maxspeed = 1f;
        public float SlowTime = 0f;
        public float Coeff = 1f;    //damage coefficient
        float speed;
        float accumulatedDamage = 0f;
        Vector3 _moveDirection = Vector3.down;
        [SerializeField] TextMeshProUGUI resourceText;

        void Start()
        {
            resourceText.text = Mathf.CeilToInt(accumulatedDamage).ToString();
            speed = Maxspeed;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false)
        {
            dmg *= Coeff;
            accumulatedDamage += dmg;
            EffectManager.SetDamageEffect(transform.position, dmg, critical);
            if (critical && AbilityManager.psychosense)
            {
                GameObject fireball = PoolManager.Get(PoolNumber.Fireball);
                fireball.transform.position = Player.transform.position;
                fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                fireball.GetComponent<Fireball>().Damage = 3f;
            }

            var p = PoolManager.Get(PoolNumber.Effect);
            p.transform.position = transform.position;

            resourceText.text = "" + Mathf.CeilToInt(accumulatedDamage);
            if (!mute)
            {
                SfxManager.PlayEnemySound(isCritical: critical);
            }
            return false;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Area"))
            {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.Slow)
                {
                    speed = Maxspeed * 0.75f;
                }
                if (collidedarea.Damage)
                {
                    accumulatedDamage -= Time.deltaTime;
                }
            }
            else if (collision.transform.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                GameManager.Instance.ActiveEnemyNum -= 1;
                GameManager.Instance.GameClear(Mathf.CeilToInt(accumulatedDamage * 0.01f));
                GameStateManager.Instance.SetState(GameState.Paused);
                Instantiate(GameClearDisplay, Vector3.zero, Quaternion.identity);
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Area"))
            {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.Slow)
                {
                    speed = Maxspeed * 0.75f;
                }
                if (collidedarea.Damage)
                {
                    accumulatedDamage -= Time.deltaTime;
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Area"))
            {
                speed = Maxspeed;
            }
        }

        void Update()
        {
            if (GameStateManager.Instance.IsPlaying)
            {
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
                transform.Translate(_moveDirection * speed * Time.deltaTime);
            }
        }
    }
}
