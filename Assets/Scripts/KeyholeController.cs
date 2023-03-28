using UnityEngine;

public class KeyholeController : MonoBehaviour
{
    public AnimationCurve MappingCurve;
    public float Amplitude = 1;
    private Camera controlledCamera;

    // Update is called once per frame
    void Update()
    {
        var c = ContextualInputSystem.Context;
        if (c == null) return;
        controlledCamera = c.GameplayCamera;
        Vector2 normalized = ContextualInputSystem.ViewNormalizedCursorPos;
        controlledCamera.transform.localPosition = new Vector3(Map(normalized.x), Map(normalized.y), 0);
    }

    private float Map(float cursorPos)
    {
        float clamped = Mathf.Clamp01(cursorPos);
        return MappingCurve.Evaluate(clamped) * Amplitude;
    }
}
