using System;
using System.Collections.Generic;
using StableFluids;
using UnityEngine;

public class AliceCharacterMovement : LaneEntity
{
    [NonSerialized] public RabbitHoleDisplay laneContext;
    private RabbitHoleDisplay prevLaneContext;

    // inspector
    public float CharacterWidth = 1.5f;
    public SpriteRenderer spriteRenderer;
    public float flipAnimationSpeed = 20;
    public float laneChangeSpeed = 2;
    public float invincibilityTime = 2f;

    // where the cursor would be if it were in the world 
    // shown in the (raycast-hit) viewport
    private Vector3 viewWorldCursorPos; 

    public virtual void Update()
    {
        if (GM.IsGameplayPaused) return;

        // process input
        ProcessInput();

        if (laneContext == null) return;

        // switch viewport
        if (prevLaneContext != laneContext)
        {
            prevLaneContext = laneContext;
            // position in lane
            transform.position = viewWorldCursorPos;
        }
        // same viewport
        else
        {
            // animate lane change
            // position in lane
            transform.position = viewWorldCursorPos;
        }

        // update animations
        //HandleFlip(dir);
    }

    //private RaycastHit[] raycastHits = new RaycastHit[1];
    private void ProcessInput()
    {
        // compare the mouse position against every display
        PerFrameVariableWatches.SetDebugQuantity("mouse", Input.mousePosition.ToString());
        var cam = GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();
        foreach (var viewport in FindObjectsOfType<RabbitHoleDisplay>())
        {
            Vector2 normalizedCursorPos = GetNormalizedCursorPos(cam, viewport);
            if (IsInBounds(normalizedCursorPos))
            {
                laneContext = viewport;
                viewWorldCursorPos = GetCursorViewportWorldPos(viewport, normalizedCursorPos);
                break;
            }
        }
    }

    private static bool IsInBounds(Vector2 norm)
    {
        return norm.x >= 0 && norm.x <= 1 && norm.y >= 0 && norm.y <= 1;
    }

    private Vector2 GetNormalizedCursorPos(Camera finalCam, RabbitHoleDisplay viewportUI)
    {
        // mouse pos
        PerFrameVariableWatches.SetDebugQuantity("mouse", finalCam.ScreenToViewportPoint(Input.mousePosition).ToString());
        // rect pos
        var rt = viewportUI.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, finalCam, out Vector2 localPos);
        // rect pos to norm viewportCam pos
        Vector2 posInViewport = new Vector2((localPos.x + rt.rect.width*.5f) / (rt.rect.width), (localPos.y + rt.rect.height * .5f) / (rt.rect.height));
        PerFrameVariableWatches.SetDebugQuantity("posInViewport", posInViewport.ToString());
        return posInViewport;
    }

    private Vector3 GetCursorViewportWorldPos(RabbitHoleDisplay rabbitHoleDisplay, Vector2 cursorViewportPos)
    {
        // norm viewportCam pos to world* pos
        Vector3 gameplayPos = rabbitHoleDisplay.GameplayCamera.ViewportToWorldPoint(cursorViewportPos);
        PerFrameVariableWatches.SetDebugQuantity("gameplayPos", gameplayPos.ToString());
        gameplayPos.z = rabbitHoleDisplay.ObstacleContext.transform.position.z; // z pos
        return gameplayPos;
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
    public override void OnDrawGizmos()
    {
        // unused
    }

#endif
}
