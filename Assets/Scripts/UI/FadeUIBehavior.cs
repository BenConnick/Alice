using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUIBehavior : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeInWithCallback(float duration, Action onComplete)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        StartCoroutine(FadeIn(duration, onComplete));
    }

    public void FadeOutWithCallback(float duration, Action onComplete)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        StartCoroutine(FadeOut(duration, onComplete));
    }

    private IEnumerator FadeIn(float duration, Action onComplete)
    {
        yield return FadeInOrOut(duration, onComplete, true);
    }

    private IEnumerator FadeOut(float duration, Action onComplete)
    {
        yield return FadeInOrOut(duration, onComplete, false);
    }

    private IEnumerator FadeInOrOut(float duration, Action onComplete, bool fadeIn)
    {
        if (duration == 0) duration = 1f;
        canvasGroup.alpha = fadeIn ? 0 : 1;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            t = fadeIn ? t : 1 - t;

            // stepped fade
            const int steps = 3;
            t = Mathf.Ceil(t * steps) / (float)steps;

            canvasGroup.alpha = t;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = fadeIn ? 1 : 0;
        onComplete?.Invoke();
    }
}
