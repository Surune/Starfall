using UnityEngine;
using Starfall.Manager;

namespace Starfall.Entity {
    public class WingBullet : MonoBehaviour {
        private Vector3 worldPos;
        public static float speed = 20f;
        public static float damage = 1f;
        
        public static bool udo = false;
        public static bool freezing = false;

        private bool gamePlaying = true;

        void Start() {
            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
            speed = 20f;
            damage = 1f;
            udo = false;
            freezing = false;
        }

        void OnDestroy() {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.transform.tag == "Enemy") {
                collision.gameObject.GetComponent<Enemy>().GetDamage(damage, critical : false, mute : true);
                if (freezing) collision.gameObject.GetComponent<Enemy>().slowTime = 2f;
                gameObject.SetActive(false);
            }
            else if (collision.transform.tag == "Boss") {
                collision.gameObject.GetComponent<Boss>().GetDamage(damage, critical : false);
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
            }
        }

        private void OnGameStateChanged(GameState newGameState) {
            gamePlaying = (newGameState == GameState.Gameplay);
        }
    }
}