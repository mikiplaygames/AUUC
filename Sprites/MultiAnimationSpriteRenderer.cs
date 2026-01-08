using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MultiAnimationSpriteRenderer : MultiAnimationAAS
{
    private SpriteRenderer spriteRenderer;

    protected override Sprite sprite
    {
        get => spriteRenderer.sprite;
        set => spriteRenderer.sprite = value;
    }
    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}