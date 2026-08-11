using UnityEngine;
using TMPro;
using Starfall.Manager;

namespace Starfall.Effect
{
    public class DamageEffect : MonoBehaviour
    {
        static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;

        [SerializeField] TMP_Text ResourceText;
        private float accumulatedTime;
        private float delay;

        public void SetEffectText(string text, Color color)
        {
			delay = 0.25f;
            accumulatedTime = 0f;
            ResourceText.text = text;
            ResourceText.color = color;
        }

        private void Update()
        {
            if (!gameObject.activeSelf || !GameStateManager.IsPlaying)
            {
                return;
            }

            accumulatedTime += Time.deltaTime;
            if (accumulatedTime > delay)
            {
                gameObject.SetActive(false);
            }
            else
            {
                var orig = ResourceText.color;
                ResourceText.color = new Color(orig.r, orig.g, orig.b, 1 - (accumulatedTime / delay));
            }
        }
    }
}
