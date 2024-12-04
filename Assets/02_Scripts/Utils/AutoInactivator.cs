using UnityEngine;

namespace Starfall.Utils {
    public class AutoInactivator : MonoBehaviour {
        [SerializeField] private float cooltime;

        void Start() {
            Invoke("destroythis", cooltime);
        }

        private void OnEnable() {
            Invoke("destroythis", cooltime);
        }

        void destroythis() {
            gameObject.SetActive(false);
        }
    }
}
