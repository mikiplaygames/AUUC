using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedImage : AnimatedSprite
{
    private Image image; public Image Image => image;
    protected override Sprite sprite { get => image.sprite; set => image.sprite = value; }

    protected void Awake()
    {
        image = GetComponent<Image>();
    }
}
