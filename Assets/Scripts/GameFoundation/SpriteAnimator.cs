using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : AbstractSpriteAnimator
{

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void SetFrame(int frame)
    {
        currentFrame = frame;
        spriteRenderer.sprite = currentAnimation.Value.Frames[currentFrame];
    }
}
