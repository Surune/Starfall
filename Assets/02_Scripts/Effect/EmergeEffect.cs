using UnityEngine;

namespace Starfall.Effect {
    public class EmergeEffect : MonoBehaviour {
        private float time = 0;
        [SerializeField] private float delayTime = 3f;

        void Start() {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }

        void Update() {
            if (time < delayTime){
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, time/delayTime);
                time += time + Time.deltaTime;
            }
        }
    }
}