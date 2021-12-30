using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteSequence[] animations;

    private SpriteSequence? currentAnimation;
    private int currentFrame;
    private float timer;

    void Update()
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
            spriteRenderer.sprite = anim.Frames[currentFrame];
        }
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
        Debug.LogWarning("Missing animation: '" + name + "'");
    }

    public void SetAnimationByIndex(int index, int startingFrameNum=0)
    {
        currentAnimation = animations[index];
        currentFrame = startingFrameNum;
        timer = 0;
        spriteRenderer.sprite = currentAnimation.Value.Frames[currentFrame];
    }
}
