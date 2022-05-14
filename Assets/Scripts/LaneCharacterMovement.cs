using System;
using System.Collections.Generic;
using StableFluids;
using UnityEngine;

public class LaneCharacterMovement : LaneEntity
{
    [SerializeField] private RabbitHole laneConfig;

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

    private void OnEnable()
    {
        Lane = GetMouseLane();
        transform.position = new Vector3(LaneUtils.GetWorldPosition(this), transform.position.y, transform.position.z);
    }

    public virtual void Update()
    {
        if (GM.IsGameplayPaused) return;

        // process input
        int dir = GetMouseLane() - Lane;
        if (dir != 0)
        {
            lastAcceptedInputTime = Time.time;
            TryChangeLane(dir);
        }

        // animate lane change
        // position in lane
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, LaneUtils.GetWorldPosition(this), laneChangeSpeed * 0.17f),
            transform.position.y,
            0);


        // update animations
        //HandleFlip(dir);
    }

    private int GetMouseLane()
    {
        Camera mainCam = GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();
        float worldX = mainCam.ScreenToWorldPoint(Input.mousePosition).x;
        float adjustedWorldX = worldX - mainCam.transform.position.x;
        float innerOrthoSize = GM.FindSingle<GameplayInnerDisplayCamera>().GetComponent<Camera>().orthographicSize;
        float camOrthoSize = mainCam.orthographicSize;
        float quadSize = GM.FindSingle("GameplayDisplayQuad").transform.localScale.y * .5f;
        int effectiveMouseLane = LaneUtils.GetLanePosition(this, adjustedWorldX * (innerOrthoSize / camOrthoSize) * (quadSize / camOrthoSize) + LaneUtils.LaneScale * .5f);
        int roundedMouseLane = effectiveMouseLane - (effectiveMouseLane % WidthLanes);
        return roundedMouseLane;
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

    private bool TryChangeLane(int direction)
    {
        if (direction == 0) return false;
        int totalLanes = LaneUtils.NumLanes;
        int width = WidthLanes;
        int prevLane = Lane;
        int newLane = direction + prevLane;
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
    }
#endif
}
