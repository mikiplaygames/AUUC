using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSpriteRenderer : AnimatedSprite
{
    private SpriteRenderer spriteRenderer; public SpriteRenderer SpriteRenderer => spriteRenderer;

    protected override Sprite sprite { get => spriteRenderer.sprite; set => spriteRenderer.sprite = value; }

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
