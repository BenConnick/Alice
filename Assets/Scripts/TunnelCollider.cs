using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelCollider : MonoBehaviour
{
    public float loopDistance = 22.8f;

    // Update is called once per frame
    void Update()
    {
        Vector3 parentPos = transform.parent.localPosition;
        float cycles = Mathf.Floor(parentPos.y / loopDistance);
        transform.localPosition = new Vector3(0, -cycles * loopDistance, 0);
    }
}
