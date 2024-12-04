using UnityEngine;

namespace Starfall.Utils {
    public class AutoRotate : MonoBehaviour {
        [SerializeField] private bool fixSpeed;
        [SerializeField] private float rotateSpeed;

        void Start() {
            if (!fixSpeed) {
                rotateSpeed = Random.value * 50f;
            }
            if (Random.value > 0.5f) rotateSpeed = -rotateSpeed;
        }

        void Update() {
            transform.Rotate(new Vector3(0, 0, 1f) * Time.deltaTime * rotateSpeed);
        }
    }
}