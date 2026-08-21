using UnityEngine;
using TMPro;
using Audio;
using Core.Constants;
using Gameplay.Managers;
using Gameplay.Spawning;

namespace Gameplay.Entities
{
    public class Boss : MonoBehaviour, IDependencyInjectable, IDamageable
    {
        private EffectManager effectManager;
        private AbilityManager abilityManager;
        private PoolManager poolManager;
        private SoundManager sound;
        private Player player;
        private Spawner spawner;
        private GameStateManager gameStateManager;
        private System.Action enemyRemoved;
        private System.Action<int> gameCompleted;
        
        public GameObject GameClearDisplay;
        public bool IsBoss => true;
        public float Maxspeed = 1f;
        public float SlowTime = 0f;
        public float Coeff = 1f;    //damage coefficient
        float speed;
        float accumulatedDamage = 0f;
        Vector3 _moveDirection = Vector3.down;
        [SerializeField] TMP_Text resourceText;

        public void InjectDependency(GameDependencies dependencies)
        {
            effectManager = dependencies.EffectManager;
            abilityManager = dependencies.AbilityManager;
            poolManager = dependencies.PoolManager;
            sound = dependencies.SoundManager;
            player = dependencies.Player;
            spawner = dependencies.Spawner;
            gameStateManager = dependencies.GameStateManager;
            enemyRemoved = dependencies.EnemyRemoved;
            gameCompleted = dependencies.GameCompleted;
        }

        private void Start()
        {
            resourceText.text = Mathf.CeilToInt(accumulatedDamage).ToString();
            speed = Maxspeed;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false)
        {
            dmg *= Coeff;
            accumulatedDamage += dmg;
            effectManager.SetDamageEffect(transform.position, dmg, critical);

            resourceText.text = "" + Mathf.CeilToInt(accumulatedDamage);
            if (!mute)
            {
                sound.PlaySFX(SoundKey.EnemyCritical);
            }
            return false;
        }

        public void ApplyDamage(float damage, bool critical = false, bool mute = false, bool fatal = false)
        {
            GetDamage(damage, critical, mute);
        }

        public void ApplySlow(float duration)
        {
            SlowTime = duration;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                spawner.RemoveActiveTarget(transform);
                enemyRemoved();
                gameCompleted(Mathf.CeilToInt(accumulatedDamage * 0.01f));
                gameStateManager.SetState(GameState.Paused);
                Instantiate(GameClearDisplay, Vector3.zero, Quaternion.identity);
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
            transform.Translate(_moveDirection * speed * Time.deltaTime);
        }
    }
}
