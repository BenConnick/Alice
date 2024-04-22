using System;
using System.Collections;
using UnityEngine;

public static class TimeDistortionController
{
    public static float BaselineSpeed { get; private set; } = 1f;

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return World.StartGlobalCoroutine(routine);
    }

    public static void SlowResume()
    {
        StartCoroutine(LerpTimescaleCoroutine(defaultResumeDuration, 0.01f, BaselineSpeed));
    }

    public static void PlayImpactFrame(float duration = 0.2f)
    {
        StartCoroutine(HitFrame(duration));
    }

    public static void PlaySlowmoAndResume()
    {
        StartCoroutine(ArcLerpTimescaleCoroutine(defaultResumeDuration, 0.1f, BaselineSpeed));
    }

    public static void PlaySlowmoWithCallback(Action callback)
    {
        StartCoroutine(LerpTimescaleCoroutine(defaultResumeDuration, 0.1f, 0.01f, callback));
    }

    public static void PlayRewind()
    {
        StartCoroutine(RewindCoroutine(defaultResumeDuration));
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
    private const float defaultResumeDuration = 1f;
    private static IEnumerator LerpTimescaleCoroutine(float resumeDuration, float initial, float final, Action callback = null)
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
        Time.timeScale = BaselineSpeed;
        callback?.Invoke();
        //Debug.Log("timescale " + Time.timeScale);
    }

    private static IEnumerator ArcLerpTimescaleCoroutine(float resumeDuration, float middle, float final)
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
        Time.timeScale = BaselineSpeed;
        //Debug.Log("timescale " + Time.timeScale);
    }

    private static IEnumerator HitFrame(float freezeDuration)
    {
        float hitStart = Time.unscaledTime;
        while (Time.unscaledTime < hitStart + freezeDuration)
        {
            Time.timeScale = 0;
            //Debug.Log("timescale " + Time.timeScale);
            yield return null;
        }
        Time.timeScale = BaselineSpeed;
        //Debug.Log("timescale " + Time.timeScale);
    }

    public static void SetBaselineSpeed(float target)
    {
        BaselineSpeed = target;
        Time.timeScale = BaselineSpeed;
    }
}
