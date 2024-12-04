using UnityEngine;
using TMPro;
using Starfall.Manager;

namespace Starfall.Effect {
    public class DamageEffect : MonoBehaviour {
        #region Manager
        private static GameStateManager gameStateManager => GameManager.Instance.gameStateManager;
        #endregion
        
        public float delay;
        public TextMeshProUGUI resourceText;
        [HideInInspector] public float accumulatedTime;

        void Update () {
            if (!gameObject.activeSelf || !gameStateManager.IsPlaying)
                return;

            accumulatedTime += Time.deltaTime;
            if (accumulatedTime > delay) {
                gameObject.SetActive(false);
            }
            else {
                var orig = resourceText.color;
                resourceText.color = new Color(orig.r, orig.g, orig.b, 1 - (accumulatedTime / delay));
            }
        }
    }
}
