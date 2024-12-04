using UnityEngine;
using Starfall.Manager;

namespace Starfall.Entity {
    public class Fireball : MonoBehaviour {
        #region Manager
        private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
        #endregion
        private Vector3 worldPos;
        public float speed = 20f;
        public float damage = 1f;
        public static float fatalDamage = 2f;
        public bool udo = false;
        public bool isCritical = false;
        public bool penetrate = false;
        public bool isFatal = false;
        public bool psychosink = false;
        public bool beingstronger = false;
        public bool burst = false;
        public bool freezing = false;

        private bool gamePlaying = true;

        void Start() {
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        void OnDestroy() {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnEnable() {
            udo = false;
            isCritical = false;
            penetrate = false;
            isFatal = false;
            psychosink = false;
            beingstronger = false;
            burst = false;
            freezing = false;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.transform.tag == "Enemy") {
                if (isFatal) {
                    if (collision.gameObject.GetComponent<Enemy>().isBoss == false) {
                        collision.gameObject.GetComponent<Enemy>().GetDamage(damage, fatal : true);
                    }
                    else {
                        collision.gameObject.GetComponent<Enemy>().GetDamage(damage * fatalDamage, fatal : false);
                    }
                }
                else if (psychosink) 
                    playerManager.DamageAllEnemy(damage);
                else
                    collision.gameObject.GetComponent<Enemy>().GetDamage(damage, critical : isCritical);
                
                if (isCritical && burst) collision.gameObject.GetComponent<Enemy>().GetDamage(playerManager.damage, mute: true);
                if (freezing) collision.gameObject.GetComponent<Enemy>().slowTime = 2f;
                if (!penetrate) {
                    gameObject.SetActive(false);
                }
                else penetrate = false;
            }
            else if (collision.transform.tag == "Boss") {
                if (isFatal) {
                    damage *= fatalDamage;
                }
                collision.gameObject.GetComponent<Boss>().GetDamage(damage, isCritical);
                
                if (isCritical && burst) collision.gameObject.GetComponent<Boss>().GetDamage(playerManager.damage, mute: true);
                if (freezing) collision.gameObject.GetComponent<Boss>().slowTime = 2f;
                gameObject.SetActive(false);
            }
        }

        void FixedUpdate () {
            worldPos = Camera.main.WorldToViewportPoint(this.transform.position);
            if (worldPos.x < 0f || worldPos.x > 1f || worldPos.y < 0f || worldPos.y > 1f) {
                gameObject.SetActive(false);
            }
            else if(gamePlaying) {
                if (udo) {
                    Transform closest = GameManager.FindClosestTransform(GameManager.GetAllChilds(GameManager.Instance.enemyList), this.transform.position);
                    if (closest != null && Vector2.Distance(this.transform.position, closest.position) < 1f)
                            transform.position = Vector3.Lerp(this.transform.position, closest.position, Time.smoothDeltaTime * speed);
                    else 
                        transform.Translate(0, Time.smoothDeltaTime * speed, 0);
                }
                else transform.Translate(0, Time.smoothDeltaTime * speed, 0);

                if (beingstronger) damage += damage * Time.smoothDeltaTime;
            }
        }

        private void OnGameStateChanged(GameState newGameState) {
            gamePlaying = (newGameState == GameState.Gameplay);
        }
    }
}