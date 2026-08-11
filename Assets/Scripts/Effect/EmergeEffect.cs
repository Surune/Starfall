using UnityEngine;

namespace Starfall.Effect
{
    public class EmergeEffect : MonoBehaviour
    {
        [SerializeField] private float delayTime = 3f;
        private float time = 0;
        private SpriteRenderer sprite;

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.clear;
        }

        private void Update()
        {
            if (time < delayTime)
            {
                sprite.color = new Color(1, 1, 1, time / delayTime);
                time += Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
