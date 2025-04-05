using UnityEngine;
using TMPro;
using Starfall.Manager;

namespace Starfall.Effect
{
    public class DamageEffect : MonoBehaviour
    {
        static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;

        [SerializeField] TextMeshProUGUI ResourceText;
        float AccumulatedTime;
		float Delay;

        public void SetEffectText(string text, Color color)
        {
			Delay = 0.25f;
            AccumulatedTime = 0f;
            ResourceText.text = text;
            ResourceText.color = color;
        }

        void Update()
        {
            if (!gameObject.activeSelf || !GameStateManager.IsPlaying)
            {
                return;
            }

            AccumulatedTime += Time.deltaTime;
            if (AccumulatedTime > Delay)
            {
                gameObject.SetActive(false);
            }
            else
            {
                var orig = ResourceText.color;
                ResourceText.color = new Color(orig.r, orig.g, orig.b, 1 - (AccumulatedTime / Delay));
            }
        }
    }
}
