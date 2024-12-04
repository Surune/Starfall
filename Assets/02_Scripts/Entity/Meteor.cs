using UnityEngine;
using Starfall.Manager;

namespace Starfall.Entity {
    public class Meteor : MonoBehaviour {
        #region Manager
        private static HPManager hpManager => GameManager.Instance.HPManager;
        #endregion
        private static Vector3 moveDirection = new Vector3(0, -1, 0);
        public float speed = 10f;
        private int damage;

        void Start() {
            damage = GameManager.Instance.timer.waveNum;
        }

        void OnEnable() {
            damage = GameManager.Instance.timer.waveNum;
        }

        void Update() {
            if (gameObject.activeSelf && GameStateManager.Instance.IsPlaying)
                transform.position += moveDirection * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.transform.tag == "Player") {
                hpManager.GetDamage(-damage);
                gameObject.SetActive(false);
            }
        }
    }
}