using UnityEngine;
using TMPro;
using Starfall.Manager;
using Starfall.Constants;
    
namespace Starfall.Entity {
    public class Boss : MonoBehaviour {
        #region Managers
        private static EffectManager effectManager => GameManager.Instance.EffectManager;
        private static AbilityManager abilityManager => GameManager.Instance.AbilityManager;
        private static PoolManager poolManager => GameManager.Instance.PoolManager;
        private static Player player => GameManager.Instance.Player;        
        #endregion
        
        private static AudioSource musicPlayer;
        public GameObject gameClearDisplay;
        [SerializeField] private TextMeshProUGUI resourceText;
        public bool isBoss = true;
        public float maxspeed = 1f;
        private float speed;
        public float slowTime = 0f;
        private float accumulatedDamage = 0f;
        public float coeff = 1f;    //damage coefficient
        private Vector3 moveDirection = new Vector3(0, -1, 0);
        [SerializeField] private AudioClip sfxHit;
        [SerializeField] private AudioClip sfxCritical;

        void Start () {
            if(musicPlayer == null)
                musicPlayer = GameObject.Find("Effects").GetComponent<AudioSource>();
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
            enabled = GameStateManager.Instance.IsPlaying;
            resourceText.text = "" + Mathf.CeilToInt(accumulatedDamage);
            speed = maxspeed;
        }

        void OnDestroy() {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        public bool GetDamage(float dmg, bool critical = false, bool mute = false) {
            dmg *= coeff;
            accumulatedDamage += dmg;
            effectManager.SetDamageEffect(transform.position, dmg, critical);
            if (critical && abilityManager.psychosense){
                GameObject fireball = poolManager.Get(PoolNumber.Fireball);
                fireball.transform.position = player.transform.position;
                fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                fireball.GetComponent<Fireball>().damage = 3f;
            }
            
            var p = poolManager.Get(PoolNumber.Effect);
            p.transform.position = transform.position;
            
            resourceText.text = "" + Mathf.CeilToInt(accumulatedDamage);
            if (!mute) {
                if (!critical) musicPlayer.PlayOneShot(sfxHit);
                else    musicPlayer.PlayOneShot(sfxCritical);
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.transform.tag == "Area") {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.slow) speed = maxspeed * 0.75f;
                if (collidedarea.damage) accumulatedDamage -= Time.deltaTime;
            }
            else if (collision.transform.tag == "Player") {
                gameObject.SetActive(false);
                GameManager.Instance.activeEnemyNum -= 1;
                GameManager.Instance.GameClear(Mathf.CeilToInt(accumulatedDamage * 0.01f));
                GameStateManager.Instance.SetState(GameState.Paused);
                Instantiate(gameClearDisplay, new Vector3(0f, 0f, 0f), Quaternion.identity);
            }
        }
        
        private void OnTriggerStay2D(Collider2D collision) {
            if (collision.transform.tag == "Area") {
                var collidedarea = collision.gameObject.GetComponent<Area>();
                if (collidedarea.slow) speed = maxspeed * 0.75f;
                if (collidedarea.damage) accumulatedDamage -= Time.deltaTime;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.transform.tag == "Area")
                speed = maxspeed;
        }

        void Update () {
            if (slowTime > 0f) {
                slowTime -= Time.deltaTime;
                speed = maxspeed * 0.75f;
                if (slowTime <= 0f) {
                    slowTime = 0f;
                    speed = maxspeed;
                }
            }
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }

        private void OnGameStateChanged(GameState newGameState) {
            enabled = (newGameState == GameState.Gameplay);
        }
    }
}