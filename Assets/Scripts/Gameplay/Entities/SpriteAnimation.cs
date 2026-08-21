using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Entities
{
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> sprites;
    private const float SpriteAnimationInterval = 0.1f;
    private int spriteIndex;
    private float spriteAnimationElapsed;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprites(List<Sprite> newSprites)
    {
        sprites = newSprites;
        spriteIndex = 0;
        spriteAnimationElapsed = 0f;
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void Update()
    {
        spriteAnimationElapsed += Time.deltaTime;
        if (!(spriteAnimationElapsed >= SpriteAnimationInterval))
        {
            return;
        }
        
        spriteAnimationElapsed -= SpriteAnimationInterval;
        spriteIndex = (spriteIndex + 1) % sprites.Count;
        spriteRenderer.sprite = sprites[spriteIndex];
    }
}
}
