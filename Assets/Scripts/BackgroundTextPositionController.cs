using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTextPositionController : MonoBehaviour
{
    public Transform ParentParallaxTransform;
    public float ParallaxCoefficient=1;

    [Range(0,10)]
    public float MousePositionParallaxAmount;

    private Vector3 startingLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // parallax
        Vector3 newPosition = ParentParallaxTransform.localPosition * ParallaxCoefficient;

        if (MousePositionParallaxAmount > 0)
        {
            Vector2 cursor = ContextualInputSystem.ViewNormalizedCursorPos;
            cursor -= new Vector2(.5f, .5f);
            newPosition += -MousePositionParallaxAmount * new Vector3(cursor.x, cursor.y, 0);
        }

        transform.localPosition = startingLocalPosition + newPosition;
    }
}
