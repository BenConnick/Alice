using System;
using System.Collections.Generic;
using StableFluids;
using UnityEngine;

public class AliceCharacterMovement : LaneEntity
{
    [NonSerialized] public RabbitHoleDisplay laneContext;

    // inspector
    public float CharacterWidth = 1.5f;
    public SpriteRenderer spriteRenderer;
    public float flipAnimationSpeed = 20;
    public float laneChangeSpeed = 2;
    public float invincibilityTime = 2f;

    private float laneChangeDirection;
    private bool shouldFlip;
    private const float inputCooldown = 0.07f;
    private float lastAcceptedInputTime;
    private Vector3 viewWorldCursorPos; // the position in the world where the cursor would be if it were part of the scene that is displayed in the viewport 

    private void OnEnable()
    {
    }

    public virtual void Update()
    {
        if (GM.IsGameplayPaused) return;

        // process input
        ProcessInput();

        if (laneContext == null) return;
        // animate lane change
        // position in lane
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, laneContext.GetLaneCenterWorldPos(Lane), laneChangeSpeed * 0.17f),
            viewWorldCursorPos.y,
            viewWorldCursorPos.z);

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
                PerFrameVariableWatches.SetDebugQuantity("ray " + viewport.GetInstanceID(), viewportDist);

                // sort by the display closest to the camera along the camera's forward vector
                if (closest == null || Vector3.Dot(closest.DisplayShape.position - raycastCam.transform.position, ray.direction) > viewportDist)
                {
                    closest = viewport;
                }
            }
        }
        if (closest != null)
        {
            PerFrameVariableWatches.SetDebugQuantity("closest: ", closest.GetInstanceID());
            laneContext = closest;
            viewWorldCursorPos = GetCharacterTargetPosition(raycastCam, laneContext);

            int lane = laneContext.GetLane(viewWorldCursorPos.x);
            bool changed = TryChangeLane(lane);
            if (changed) lastAcceptedInputTime = Time.time;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction);
        }
    }

    private Vector3 GetCharacterTargetPosition(Camera finalCam, RabbitHoleDisplay viewportQuad)
    {
        debugPoints.Clear();
        // coordinate
        // mouse pos to worldPos
        AddToDebugViewportQueue(finalCam.ScreenToViewportPoint(Input.mousePosition));
        Vector3 worldPos = finalCam.ScreenToWorldPoint(Input.mousePosition);
        // worldPos to normalized quad pos
        // get quad bounds
        // (quad has dimensions of 1x1, so we can use the scale to get the size)
        Vector3 quadCenter = viewportQuad.transform.position;
        Vector3 quadScale = viewportQuad.transform.lossyScale;
        float xRelative = (worldPos.x - quadCenter.x) + quadScale.x * .5f;
        float yRelative = (worldPos.y - quadCenter.y) + quadScale.y * .5f;
        Vector2 posInQuad = new Vector2(xRelative / quadScale.x, yRelative / quadScale.y); // normalized
        AddToDebugViewportQueue(posInQuad);
        // norm quad pos to norm viewportCam pos
        Vector2 posInViewport = Vector2.Scale(posInQuad, viewportQuad.AssociatedMaterial.mainTextureScale) - viewportQuad.AssociatedMaterial.mainTextureOffset;
        AddToDebugViewportQueue(posInViewport);
        // norm viewportCam pos to world* pos
        Vector3 gameplayPos = viewportQuad.GameplayCamera.ViewportToWorldPoint(posInViewport);
        gameplayPos.z = viewportQuad.ObstacleContext.transform.position.z; // z pos
        return gameplayPos;
    }

    private List<Vector2> debugPoints = new List<Vector2>();
    private void AddToDebugViewportQueue(Vector2 normalizedPoint)
    {
        debugPoints.Add(normalizedPoint);
    }

    public enum DirectionInput {
        Default,
        Left,
        Right,
        Up,
        Down,
        Selection
    }

    private bool GetDirectionInputDown(DirectionInput controlInput)
    {
        switch (controlInput)
        {
            case DirectionInput.Left:
                return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            case DirectionInput.Right:
                return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
            case DirectionInput.Up:
                return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            case DirectionInput.Down:
                return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            case DirectionInput.Selection:
                return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);
        }
        return false;
    }

    // gets the input on this frame
    private int GetHorizInput()
    {
        bool left = GetDirectionInputDown(DirectionInput.Left);
        bool right = GetDirectionInputDown(DirectionInput.Right);
        // left and right before up and down
        if ((left || right) && !(left && right)) // left or right pressed but not both
        {
            return left ? -1 : 1;
        }
        // no direction
        return 0;
    }

    private bool TryChangeLane(int newLane)
    {
        int totalLanes = LaneUtils.NumLanes;
        int width = WidthLanes;
        int prevLane = Lane;
        if (newLane < 0) newLane = 0;
        if (newLane + width >= totalLanes) newLane = totalLanes - width;
        Lane = newLane;
        return newLane != prevLane;
    }

    private void HandleFlip(float inputX)
    {
        if (inputX > 0.01f)
        {
            shouldFlip = true;
        }
        else if (inputX < -0.01f)
        {
            shouldFlip = false;
        }
        float target = shouldFlip ? -1 : 1;
        float percentToTarget = Mathf.InverseLerp(-target, target, transform.localScale.x);
        float easing = 0.5f + 0.5f * ((percentToTarget - 0.5f) * (percentToTarget - 0.5f));
        float xScale = Mathf.Clamp(transform.localScale.x + target * SmokeRendering.FixedTimeInterval * flipAnimationSpeed * easing, -1, 1);
        transform.localScale = new Vector3(xScale, 1, 1);
    }

    public void Push(Vector2 push)
    {
        // unused
    }

    public void PauseInput(float seconds)
    {
        lastAcceptedInputTime = Time.time + seconds;
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
        // update lane
        if (!Application.isPlaying && AutoLane)
        {
            var so = new UnityEditor.SerializedObject(this);
            var laneProp = so.FindProperty("Lane");
            laneProp.intValue = LaneUtils.GetLanePosition(this);
            if (laneProp.intValue != Lane && so.hasModifiedProperties) so.ApplyModifiedProperties();
        }

        // get lane data
        Vector3 center = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float w = LaneUtils.LaneScale * CharacterWidth;
        Vector3 extents = new Vector3(w, Height, 0);

        // draw
        Color prev = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, extents);
        Gizmos.color = prev;

        // draw lanes
        Gizmos.color = Color.cyan;
        for (int i = 0; i <= LaneUtils.NumLanes; i++)
        {
            float laneX = LaneUtils.GetLaneCenterWorldPosition(i);
            Gizmos.DrawLine(new Vector3(laneX, transform.position.y - 10, 0), new Vector3(laneX, transform.position.y + 10, 0));
        }

        DrawViewportDebugPoints();
    }

    private void DrawViewportDebugPoints()
    {
        // draw debug points
        Vector2 boxPos = new Vector2(10, 10);
        Vector2 boxSize = new Vector2(10, 10);
        Vector3 pointSize = new Vector3(.5f, .5f, .5f);
        foreach (var point in debugPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(boxPos, boxSize);

            Gizmos.color = Color.yellow;
            Vector2 pointPos = boxPos + Vector2.Scale(point, boxSize) - boxSize * .5f;
            Gizmos.DrawCube(pointPos, pointSize);

            // move box pos
            boxPos += new Vector2(0, -boxSize.y * 1.1f);
        }
    }

#endif
}
