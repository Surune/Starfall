using UnityEngine;
using TMPro;
using Starfall.Manager;

namespace Starfall.Effect
{
    public class DamageEffect : MonoBehaviour
    {
        static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;

        public float Delay;
        public TextMeshProUGUI ResourceText;
        public float AccumulatedTime;

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
