using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new AnimatedSpriteAsset", menuName = "AnimatedSpriteAsset")]
public class AnimatedSpriteAsset : ScriptableObject
{
    [SerializeField] private AnimatedSpriteFrame[] frames;
    [SerializeField] private bool loop = true;
    [Header("WARNING! Setting FrameTime overrides Animation Settings")]
    [SerializeField] float frameDuration = 0.1f;
    [SerializeField] bool set;
    public AnimatedSpriteFrame[] Frames => frames;
    public int Length => frames.Length;
    public bool Loop => loop;
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!set) return;
        set = false;
        for (int i = 0; i < frames.Length; i++)
        {
            frames[i].duration = frameDuration;
        }
    }
#endif
}
[Serializable]
public struct AnimatedSpriteFrame
{
    public Sprite sprite;
    public float duration;
}