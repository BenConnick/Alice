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
        if (duration == 0) duration = 1f;
        canvasGroup.alpha = 0;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = t;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 1;
        onComplete?.Invoke();
    }

    private IEnumerator FadeOut(float duration, Action onComplete)
    {
        if (duration == 0) duration = 1f;
        canvasGroup.alpha = 1;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = 1 - t;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 0;
        onComplete?.Invoke();
    }
}
