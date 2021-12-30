using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float speed = 1;

    private Vector2 inputVector = new Vector2();

    void Update()
    {
        // process input
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // move
        if (inputVector.sqrMagnitude > 0)
        {
            var normalized = inputVector.normalized;
            transform.position += speed * Time.deltaTime * new Vector3(normalized.x, normalized.y, 0);
        }

        // update animations
        if (inputVector.x > 0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else if (inputVector.x < -0.01f)
        {
            spriteRenderer.flipX = false;
        }
    }
}
