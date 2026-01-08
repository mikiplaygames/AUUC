using UnityEngine;

public abstract class MultiAnimationAAS : AnimatedSprite
{
    [SerializeField] protected AnimatedSpriteAsset[] spritesList;
    private int currentAnimationIndex = 0;
    public void SetCurrentAnimation(int index)
    {
        if (index < 0 || index >= spritesList.Length) return;
        Play(spritesList[index], IsPlaying);
    }
    public void NextAnimation()
    {
        currentAnimationIndex = (currentAnimationIndex + 1) % spritesList.Length;
        Play(spritesList[currentAnimationIndex], IsPlaying);
    }
    public void PreviousAnimation()
    {
        currentAnimationIndex = (currentAnimationIndex - 1 + spritesList.Length) % spritesList.Length;
        Play(spritesList[currentAnimationIndex], IsPlaying);
    }
}
