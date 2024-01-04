using UnityEngine;
using System.Collections;
using System;

public abstract class AbstractSpriteAnimator : MonoBehaviour
{
    [Serializable]
    public struct SpriteSequence
    {
        public string Name;
        public Sprite[] Frames;
        public float Duration;
        public bool Looping;
        public int LoopFrame;
    }

    [SerializeField] protected SpriteSequence[] animations;
    [SerializeField] protected string Autoplay;

    protected SpriteSequence? currentAnimation;
    protected int currentFrame;
    protected float timer;

    protected virtual void Start()
    {
        if (Autoplay != null)
        {
            SetAnimation(Autoplay);
        }
    }

    protected virtual void Update()
    {
        if (currentAnimation.HasValue)
        {
            var anim = currentAnimation.Value;
            int numFrames = anim.Frames.Length;
            timer += Time.deltaTime;
            float segmentLength = anim.Duration / numFrames;
            if (timer > segmentLength)
            {
                timer -= segmentLength;
                currentFrame++;
                if (currentFrame >= numFrames)
                {
                    currentFrame = anim.Looping ? anim.LoopFrame : numFrames-1;
                }
            }
            SetFrame(currentFrame);
        }
    }

    public string GetAnimation()
    {
        if (currentAnimation == null) return "";
        return currentAnimation?.Name;
    }

    public void SetAnimation(string name, int startingFrameNum=0)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (string.CompareOrdinal(animations[i].Name, name) == 0)
            {
                SetAnimationByIndex(i);
                return;
            }
        }
        Debug.LogWarning(gameObject.name + " missing animation: '" + name + "'");
    }

    public void SetAnimationByIndex(int index, int startingFrameNum=0)
    {
        currentAnimation = animations[index];
        timer = 0;
        SetFrame(startingFrameNum);
    }

    protected abstract void SetFrame(int frame);
}
