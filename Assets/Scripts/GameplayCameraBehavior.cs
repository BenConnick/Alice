using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameplayCameraBehavior : MonoBehaviour
{
    public float shakeHeight = 1;
    public float shakeWavelength = 1;
    public float defaultShakeDuration = 1;

    private Coroutine shakeAnimation;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Shake(float? seconds=null)
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
        float startTime = Time.time;
        float remaining = seconds;
        while(remaining > 0)
        {
            yield return null;
            remaining = startTime + seconds - Time.time;
            float cycle = (remaining % shakeWavelength) / shakeWavelength; // 0 to 1
            float pattern = Mathf.Sin(cycle); // up and back down like a sin wave
            float y = pattern * shakeHeight;
            cam.orthographicSize = startValue + y * remaining/seconds;
        }
        cam.orthographicSize = startValue;
        shakeAnimation = null; // remove own ref
    }
}
