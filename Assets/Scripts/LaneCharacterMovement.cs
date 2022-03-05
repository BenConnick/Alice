using StableFluids;
using UnityEngine;

public class LaneCharacterMovement : LaneEntity
{
    [SerializeField] private RabbitHole laneConfig;

    // inspector
    public SpriteRenderer spriteRenderer;
    public float flipAnimationSpeed = 20;
    public float laneChangeSpeed = 2;

    private float laneChangeDirection;
    private bool shouldFlip;
    private float inputCooldown;

    void Update()
    {
        if (GM.IsGameplayPaused) return;

        // process input
        int dir = GetHorizInput();
        TryChangeLane(dir);

        // animate lane change
        // position in lane
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, LaneUtils.GetWorldPosition(this), laneChangeSpeed * 0.17f),
            transform.position.y,
            0);


        // update animations
        HandleFlip(dir);
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
                return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            case DirectionInput.Right:
                return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
            case DirectionInput.Up:
                return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
            case DirectionInput.Down:
                return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
            case DirectionInput.Selection:
                return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
        }
        return false;
    }

    /* unused
    private DirectionInput GetInputDirection()
    {
        bool left = GetDirectionInputDown(DirectionInput.Left);
        bool right = GetDirectionInputDown(DirectionInput.Right);
        bool up = GetDirectionInputDown(DirectionInput.Up);
        bool down = GetDirectionInputDown(DirectionInput.Down);
        // left and right before up and down
        if ((left || right) && !(left && right)) // left or right pressed but not both
        {
            return left ? DirectionInput.Left : DirectionInput.Right;
        }
        // up and down
        else
        {
            if (up)
            {
                return DirectionInput.Up;
            }
            else if (down)
            {
                return DirectionInput.Down;
            }
            else
            {
                return DirectionInput.Default;
            }
        }
    } */

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
        int newLane = width * direction + prevLane;
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
        inputCooldown = seconds;
    }

    public void StartFlashing()
    {
        GetComponent<FlashingBehavior>().StartFlashing();
    }
}
