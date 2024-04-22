using System;
using System.Collections.Generic;
using UnityEngine;

public class AliceCharacterMovement : MonoBehaviour
{
    public bool IsHijacked;
    public FallingGameInstance gameContext => ContextualInputSystem.ActiveGameInstance;
    private FallingGameInstance prevGameContext;

    // inspector
    public float CharacterWidth => 1.5f * transform.localScale.x;
    public SpriteRenderer spriteRenderer;
    public float flipAnimationSpeed = 20;
    public float laneChangeSpeed = 2;
    public float invincibilityTime = 2f;
    public float rotationSpeed;

    private float angle;

    public virtual void Update()
    {
        if (ApplicationLifetime.IsGameplayPaused) return;

        if (gameContext == null) return;

        // switch viewport
        if (prevGameContext != gameContext)
        {
            prevGameContext = gameContext;
            // position in lane
            transform.position = ContextualInputSystem.ViewWorldCursorPos;
        }
        // same viewport
        else
        {
            // position
            if (!IsHijacked)
            {
                const float MaxInstantMovePerSecond = 15f;
                float maxInstantMove = MaxInstantMovePerSecond * Time.deltaTime;
                Vector3 prevPos = transform.position;
                Vector3 toVec = ContextualInputSystem.ViewWorldCursorPos - prevPos;
                if (toVec.sqrMagnitude < maxInstantMove * maxInstantMove)
                {
                    // instant move to position (micro movements)
                    transform.position = ContextualInputSystem.ViewWorldCursorPos;
                }
                else
                {
                    // cap per-frame movement (macro movements)
                    transform.position = prevPos + toVec.normalized * maxInstantMove;
                }
            }
        }

        angle += Time.deltaTime * rotationSpeed;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void StartFlashing()
    {
        GetComponentCached<FlashingBehavior>().StartFlashing(invincibilityTime);
    }

    public bool IsFlashing()
    {
        return GetComponentCached<FlashingBehavior>().IsFlashing();
    }

    // too lazy to serialize, make a cache for all the components so I can use GetComponent with impunity
    private Dictionary<Type, MonoBehaviour> componentCache = new Dictionary<Type, MonoBehaviour>();
    private T GetComponentCached<T>() where T : MonoBehaviour
    {
        if(componentCache.TryGetValue(typeof(T), out MonoBehaviour monoBehaviour))
        {
            return (T)monoBehaviour;
        }
        else
        {
            var ret = GetComponent<T>();
            componentCache.Add(typeof(T), ret);
            return ret;
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        // unused
    }

#endif
}
