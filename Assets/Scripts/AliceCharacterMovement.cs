using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    public float maxInstantMovePerSecond = 15f;
    public float wallThickness = 2f;

    private float angle;

    public virtual void Update()
    {
        if (ApplicationLifetime.IsGameplayPaused) return;

        if (gameContext == null) return;

        bool mouseClick = Input.GetMouseButtonUp(0);

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
                float maxInstantMove = maxInstantMovePerSecond * Time.deltaTime;
                Vector3 viewportWorldPosition = ContextualInputSystem.ViewWorldCursorPos;
                
                // limit movement to the left and right viewport edges
                float viewportWorldHalfWidth =
                    ContextualInputSystem.ActiveGameInstance.Viewport.GameplayCamera.orthographicSize;
                viewportWorldPosition.x = Mathf.Clamp(viewportWorldPosition.x, 
                    wallThickness - viewportWorldHalfWidth,
                    viewportWorldHalfWidth - wallThickness);
                
                Vector3 prevPos = transform.position;
                Vector3 toVec = viewportWorldPosition - prevPos;
                if (toVec.sqrMagnitude < maxInstantMove * maxInstantMove || mouseClick)
                {
                    // instant move to position (micro movements)
                    transform.position = viewportWorldPosition;
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

    public void StopFlashing()
    {
        GetComponentCached<FlashingBehavior>().StopFlashing();
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

    public void GiveTemporaryInvincibility(float duration)
    {
        GetComponentCached<FlashingBehavior>().StartFlashing(duration);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        // unused
    }

#endif
}
