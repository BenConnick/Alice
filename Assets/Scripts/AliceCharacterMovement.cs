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
    public float outOfBoundsCameraPanSpeed = 10f;

    private float angle;

    public virtual void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        bool mouseClick = Input.GetMouseButtonUp(0);
        if (mouseClick && Mathf.Abs(ContextualInputSystem.ViewNormalizedCursorPos.x) > 1)
        {
            //ContextualInputSystem.AllowedOutside = true;
        }
        
        if (IsHijacked) return;
        if (ApplicationLifetime.IsGameplayPaused) return;
        
        // move outside of viewport
        if (gameContext == null && ContextualInputSystem.AllowedOutside)
        {
            UpdatePositionOutside();
        }
        else
        {
            UpdatePositionInside();
        }
    }

    private void UpdatePositionOutside()
    {
        prevGameContext = null;
        transform.position = ContextualInputSystem.ViewWorldCursorPos;
        angle += 0;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        
        PanCamera();
    }

    private void PanCamera()
    {
        float deadZoneSize = .5f;
        Vector3 renormalizedCursor = 2 * (ContextualInputSystem.ViewNormalizedCursorPos - Vector3.one * .5f);
        var cam = World.Get<GameplayCameraBehavior>().GetComponent<Camera>();
        if (Mathf.Abs(renormalizedCursor.x) < deadZoneSize && Mathf.Abs(renormalizedCursor.y) < deadZoneSize)
        {
            return;
        }
        renormalizedCursor.z = 0;
        float xSign = Mathf.Sign(renormalizedCursor.x);
        float ySign = Mathf.Sign(renormalizedCursor.y);
        renormalizedCursor.x = xSign * Mathf.InverseLerp(1 - deadZoneSize, 1, Mathf.Abs(renormalizedCursor.x));
        renormalizedCursor.y = ySign * Mathf.InverseLerp(1 - deadZoneSize, 1, Mathf.Abs(renormalizedCursor.y));
        cam.transform.localPosition += renormalizedCursor * outOfBoundsCameraPanSpeed * Time.deltaTime;
    }

    private void UpdatePositionInside()
    {
        bool sameContext = gameContext == prevGameContext;
        prevGameContext = gameContext;
        bool mouseClick = Input.GetMouseButtonUp(0);
    
        // position
        float maxInstantMove = sameContext ? maxInstantMovePerSecond * Time.deltaTime : 999; // snap if changing viewport
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
