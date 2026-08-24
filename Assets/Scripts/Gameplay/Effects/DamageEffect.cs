using UnityEngine;
using TMPro;
using Gameplay.Managers;

namespace Gameplay.Effects
{
    public class DamageEffect : MonoBehaviour, IPoolable
    {
        private GameStateManager gameStateManager;

        [SerializeField] TMP_Text ResourceText;
        private float accumulatedTime;
        private const float Delay = 0.25f;

        public void Initialize(GameStateManager gameStateManager)
        {
            this.gameStateManager = gameStateManager;
        }

        public void OnSpawn()
        {
            accumulatedTime = 0f;
        }

        public void OnDespawn()
        {
        }

        public void SetEffectText(string text, Color color)
        {
            accumulatedTime = 0f;
            ResourceText.text = text;
            ResourceText.color = color;
        }

        private void Update()
        {
            if (!gameObject.activeSelf || !gameStateManager.IsPlaying)
            {
                return;
            }

            accumulatedTime += Time.deltaTime;
            if (accumulatedTime > Delay)
            {
                gameObject.SetActive(false);
            }
            else
            {
                var orig = ResourceText.color;
                ResourceText.color = new Color(orig.r, orig.g, orig.b, 1 - (accumulatedTime / Delay));
            }
        }
    }
}
