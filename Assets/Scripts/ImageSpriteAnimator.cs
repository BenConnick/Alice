using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSpriteAnimator : AbstractSpriteAnimator
{
    private Image spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
    }

    protected override void SetFrame(int frame)
    {
        currentFrame = frame;
        spriteRenderer.sprite = currentAnimation.Value.Frames[currentFrame];
    }
}
