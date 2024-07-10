using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBlinkBehaviour : SimpleBlinkBehavior
{
    protected void OnEnable()
    {
        Transform cam = World.Get<GameplayCameraBehavior>().transform;
        time = (transform.position.y - cam.position.y) / 10f;
    }
}
