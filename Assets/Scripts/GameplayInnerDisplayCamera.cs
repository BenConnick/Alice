using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameplayInnerDisplayCamera : MonoBehaviour
{
    public const float DefaultOrthoSize = 4; public float shakeHeight = 1;
    public float shakeWavelength = 1;
    public float defaultShakeDuration = 1;

    private Coroutine shakeAnimation;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Shake(float? seconds = null)
    {
        if (shakeAnimation == null)
        {
            shakeAnimation = StartCoroutine(ShakeRoutine(seconds.GetValueOrDefault(defaultShakeDuration)));
        }
    }

    public void StopShaking()
    {
        if (shakeAnimation == null) return;
        StopCoroutine(shakeAnimation);
    }

    private IEnumerator ShakeRoutine(float seconds)
    {
        float startValue = cam.orthographicSize;
        float startTime = Time.unscaledTime;
        float remaining = seconds;
        while (remaining > 0)
        {
            yield return null;
            remaining = startTime + seconds - Time.unscaledTime;
            float cycle = (remaining % shakeWavelength) / shakeWavelength; // 0 to 1
            float pattern = Mathf.Sin(cycle); // up and back down like a sin wave
            float y = pattern * shakeHeight;
            cam.orthographicSize = startValue + y * remaining / seconds;
        }
        cam.orthographicSize = startValue;
        shakeAnimation = null; // remove own ref
    }

    public void LateUpdate()
    {
        //var alice = GM.FindSingle<Alice>();
        //float scale = alice.transform.localScale.x / Alice.DefaultScale;

        // match scale with the player and move accordingly
        //cam.orthographicSize = DefaultOrthoSize * scale;

        //transform.localScale = new Vector3(12 / (float)alice.WidthLanes, 6 / (float)alice.WidthLanes, 1);
        //Vector3 pos = alice.transform.position;
        //transform.localPosition = new Vector3(0, pos.y - pos.y * scale, -10);
    }
}
