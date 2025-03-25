using UnityEngine;

namespace Starfall.Effect
{
    public class EmergeEffect : MonoBehaviour
    {
        [SerializeField] float delayTime = 3f;
        float time = 0;
        SpriteRenderer sprite;

        void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(1, 1, 1, 0);
        }

        void Update()
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
