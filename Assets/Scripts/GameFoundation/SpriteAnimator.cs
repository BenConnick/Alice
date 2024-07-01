using System;
using UnityEngine;
using Object = System.Object;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : AbstractSpriteAnimator
{
    private SpriteRenderer spriteRenderer;

    public void Validate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Awake()
    {
        Validate();
    }

    protected override void SetFrame(int frame)
    {
        currentFrame = frame;
        spriteRenderer.sprite = currentAnimation.Value.Frames[currentFrame];
    }
}
