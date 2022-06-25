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
        int layer_mask = LayerMask.GetMask("MovieScreen");
        Camera raycastCam = GM.FindSingle("GameDisplayCamera").GetComponent<Camera>();
        // note: phyiscs raycasts behaved in a buggy way (probably because I turned off phyiscs) using ortho math instead
        Ray ray = raycastCam.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPoint = raycastCam.ScreenToWorldPoint(Input.mousePosition);
        Plane viewPlane = new Plane(ray.direction, 0);

        // compare the mouse position against every display
        RabbitHoleDisplay closest = null;
        foreach (var viewport in FindObjectsOfType<RabbitHoleDisplay>())
        {
            // what are the extents of the display
            Vector3 quadCenter = viewport.DisplayShape.position;
            Vector3 quadScale = viewport.DisplayShape.lossyScale;
            float xRelative = (worldPoint.x - quadCenter.x) + quadScale.x * .5f;
            float yRelative = (worldPoint.y - quadCenter.y) + quadScale.y * .5f;
            Vector2 posInQuad = new Vector2(xRelative / quadScale.x, yRelative / quadScale.y); // normalized
            // is the mouse position contained within those extents?
            if (posInQuad.x >= 0 && posInQuad.x <= 1 && posInQuad.y >= 0 && posInQuad.y <= 1) {
                float viewportDist = Vector3.Dot(viewport.DisplayShape.position - raycastCam.transform.position, ray.direction);
                PerFrameVariableWatches.SetDebugQuantity("ray " + viewport.GetInstanceID(), viewportDist.ToString());

                // sort by the display closest to the camera along the camera's forward vector
                if (closest == null || Vector3.Dot(closest.DisplayShape.position - raycastCam.transform.position, ray.direction) > viewportDist)
                {
                    closest = viewport;
                }
            }
        }
        if (closest != null)
        {
            PerFrameVariableWatches.SetDebugQuantity("closest: ", closest.GetInstanceID().ToString());
            laneContext = closest;
            viewWorldCursorPos = GetCursorInnerViewportPos(raycastCam, laneContext);
            PerFrameVariableWatches.SetDebugQuantity("viewWorldCursorPos", viewWorldCursorPos.ToString());
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction);
        }
    }

    private Vector3 GetCursorInnerViewportPos(Camera finalCam, RabbitHoleDisplay viewportQuad)
    {
        // coordinate
        // mouse pos to worldPos
        PerFrameVariableWatches.SetDebugQuantity("mouse", finalCam.ScreenToViewportPoint(Input.mousePosition).ToString());
        Vector3 worldPos = finalCam.ScreenToWorldPoint(Input.mousePosition);
        // worldPos to normalized quad pos
        // get quad bounds
        // (quad has dimensions of 1x1, so we can use the scale to get the size)
        Vector3 quadCenter = viewportQuad.transform.position;
        Vector3 quadScale = viewportQuad.transform.lossyScale;
        float xRelative = (worldPos.x - quadCenter.x) + quadScale.x * .5f;
        float yRelative = (worldPos.y - quadCenter.y) + quadScale.y * .5f;
        Vector2 posInQuad = new Vector2(xRelative / quadScale.x, yRelative / quadScale.y); // normalized
        PerFrameVariableWatches.SetDebugQuantity("posInQuad", posInQuad.ToString());
        // norm quad pos to norm viewportCam pos
        Vector2 posInViewport = Vector2.Scale(posInQuad, viewportQuad.AssociatedMaterial.mainTextureScale) - viewportQuad.AssociatedMaterial.mainTextureOffset;
        PerFrameVariableWatches.SetDebugQuantity("posInViewport", posInViewport.ToString());
        // norm viewportCam pos to world* pos
        Vector3 gameplayPos = viewportQuad.GameplayCamera.ViewportToWorldPoint(posInViewport);
        PerFrameVariableWatches.SetDebugQuantity("gameplayPos", gameplayPos.ToString());
        gameplayPos.z = viewportQuad.ObstacleContext.transform.position.z; // z pos
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
