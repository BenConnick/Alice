using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBlinkBehavior : MonoBehaviour
{
    public MonoBehaviour Renderer;
    public AnimationCurve BlinkCurve;
    public float LoopSeconds;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        time %= LoopSeconds;
        bool vis = BlinkCurve.Evaluate(time / LoopSeconds) > .5f;
        Renderer.enabled = vis;
    }
}
