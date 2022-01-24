using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float accelerationPower = 1;
    public float decelerationPower = 1;
    public float horizontalMaxSpeed = 1;
    public float upwardMaxSpeed = 1;
    public float downwardMaxSpeed = 1;
    public float flipAnimationSpeed = 20;

    private Vector2 velocity;
    private bool shouldFlip;
    private float inputCooldown;

    private Vector2 inputVector = new Vector2();

    void Update()
    {
        if (GM.IsGameplayPaused) return;

        // process input
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputCooldown > 0)
        {
            inputVector = Vector2.zero;
            inputCooldown -= Time.deltaTime;
        }

        // accelerate
        if (inputVector.sqrMagnitude > 0)
        {
            var normalized = inputVector.normalized;
            Vector2 acceleration = new Vector2(normalized.x, normalized.y);
            velocity += acceleration * Time.deltaTime * accelerationPower;
            velocity = new Vector2(Mathf.Clamp(velocity.x, -horizontalMaxSpeed, horizontalMaxSpeed), Mathf.Clamp(velocity.y, -downwardMaxSpeed, upwardMaxSpeed));
        }
        // decelerate
        float decelerationFactor = Mathf.Min(1, Time.deltaTime / decelerationPower);
        if (inputVector.x == 0)
        {
            float x = velocity.x * decelerationFactor;
            velocity = new Vector2(x, velocity.y);    
        }
        if (inputVector.y == 0)
        {
            float y = velocity.y * decelerationFactor;
            velocity = new Vector2(velocity.x, y);
        }

        transform.position += new Vector3(Time.deltaTime * velocity.x, Time.deltaTime * velocity.y,0);

        // update animations
        if (inputVector.x > 0.01f)
        {
            shouldFlip = true;
        }
        else if (inputVector.x < -0.01f)
        {
            shouldFlip = false;
        }
        float target = shouldFlip ? -1 : 1;
        float percentToTarget = Mathf.InverseLerp(-target, target, transform.localScale.x);
        float easing = 0.5f + 0.5f * ((percentToTarget-0.5f) * (percentToTarget-0.5f));
        float xScale = Mathf.Clamp(transform.localScale.x + target * Time.deltaTime * flipAnimationSpeed * easing,-1,1);
        transform.localScale = new Vector3(xScale, 1, 1);
    }

    public void Push(Vector2 push)
    {
        velocity = new Vector2(Mathf.Clamp(push.x, -horizontalMaxSpeed, horizontalMaxSpeed), Mathf.Clamp(push.y, -downwardMaxSpeed, upwardMaxSpeed));
        transform.position += new Vector3(Time.deltaTime * velocity.x, Time.deltaTime * velocity.y, 0);
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
