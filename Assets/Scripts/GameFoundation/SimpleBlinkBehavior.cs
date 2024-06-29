using GameFoundation;
using UnityEngine;

public class SimpleBlinkBehavior : VisualMonoBehaviour
{
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
        bool isVisible = BlinkCurve.Evaluate(time / LoopSeconds) > .5f;
        SetVisible(isVisible);
    }
}
