using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int framesPerSprite = 10;
    private int spriteIndex;
    private int frameCount;

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprites(List<Sprite> newSprites)
    {
        sprites = newSprites;
        spriteIndex = 0;
        frameCount = 0;
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void FixedUpdate()
    {
        frameCount++;
        if (frameCount > framesPerSprite)
        {
            frameCount -= framesPerSprite;
            spriteIndex = (spriteIndex + 1) % sprites.Count;
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }
}
