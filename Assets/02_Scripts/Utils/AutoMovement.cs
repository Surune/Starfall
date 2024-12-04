using UnityEngine;
using Starfall.Manager;

namespace Starfall.Utils {
    public class AutoMovement : MonoBehaviour {
        private Camera mainCamera;
        [SerializeField] private Vector3 direction;
        [SerializeField] private float speed = 1f;
        public bool continuous = false;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        void Update() {
            if(GameStateManager.Instance.IsPlaying) {
                transform.position += direction * Time.deltaTime * speed;
            }

            Vector3 objectPosition = transform.position;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(objectPosition);
            if (screenPosition.x < 0f || screenPosition.x > Screen.width ||
                    screenPosition.y < 0f || screenPosition.y > Screen.height) {
                if (continuous) {
                    // Teleport the object to the opposite side
                    Vector3 newPosition = objectPosition;
                    newPosition.x = -newPosition.x;
                    newPosition.y = -newPosition.y;
                    transform.position = newPosition;
                }
                else {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
