using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeDistortionController
{
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return GM.StartCoroutine(routine);
    }

    public static void SlowResume()
    {
        StartCoroutine(LerpTimescaleCoroutine(0.01f, 1f));
    }

    public static void PlaySlowmoAndResume()
    {
        StartCoroutine(ArcLerpTimescaleCoroutine(0.1f, 1f));
    }

    public static void PlaySlowmoWithCallback(Action callback)
    {
        StartCoroutine(LerpTimescaleCoroutine(0.1f, 0.01f, callback));
    }

    public static void PlayRewind()
    {
        StartCoroutine(RewindCoroutine(resumeDuration));
    }

    private static IEnumerator RewindCoroutine(float duration)
    {
        float rewindStart = Time.unscaledTime;
        while (Time.unscaledTime < rewindStart + duration)
        {
            float t = (Time.unscaledTime - rewindStart) / duration;
            //CameraController.PlayRewindAnimation();
            // PC.transform.localPosition += new Vector3(0, 10*Time.unscaledDeltaTime, 0);
            yield return null;
        }
        // PC.SetVelocity(new Vector2(0, 10));

        // PlatformManager.UpdateSpeedZone(PC.transform.localPosition.y, PC.transform.localPosition.y, true);
        // IsGameplayPaused = false;
    }

    private static float resumeStart;
    private const float resumeDuration = 1f;
    private static IEnumerator LerpTimescaleCoroutine(float initial, float final, Action callback = null)
    {
        resumeStart = Time.unscaledTime;
        while (Time.unscaledTime < resumeStart + resumeDuration)
        {
            float t = (Time.unscaledTime - resumeStart) / resumeDuration;
            t = t * t;
            Time.timeScale = Mathf.Lerp(initial, final, t);
            //Debug.Log("timescale " + Time.timeScale);
            yield return null;
        }
        Time.timeScale = 1;
        callback?.Invoke();
        //Debug.Log("timescale " + Time.timeScale);
    }

    private static IEnumerator ArcLerpTimescaleCoroutine(float middle, float final)
    {
        resumeStart = Time.unscaledTime;
        while (Time.unscaledTime < resumeStart + resumeDuration)
        {
            float t = (Time.unscaledTime - resumeStart) / resumeDuration;
            t = t * t - t + 0.25f;
            Time.timeScale = Mathf.Lerp(middle, final, t);
            //Debug.Log("timescale " + Time.timeScale);
            yield return null;
        }
        Time.timeScale = 1;
        //Debug.Log("timescale " + Time.timeScale);
    }
}
