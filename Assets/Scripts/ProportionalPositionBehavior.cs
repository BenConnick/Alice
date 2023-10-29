using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProportionalPositionBehavior : MonoBehaviour
{
    public Transform TrackedTransform;

    public float Ratio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = TrackedTransform.localPosition * Ratio;
    }
}
