using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlitchAnimator : MonoBehaviour
{
    public Material GlitchMaterial;
    public RawImage TextureRenderer;

    [Range(0,1)]
    public float ManualIntensity;

    private Material glitchMatInstance;

    private Coroutine activeAnimation;

    public AnimationCurve PlaceholderAnimationCurve;

    public void Awake()
    {
        glitchMatInstance = new Material(GlitchMaterial);
        TextureRenderer.material = glitchMatInstance;
    }

    public void SetIntensity(float intensity)
    {
        ManualIntensity = intensity;
        var _material = glitchMatInstance;
        _material.SetVector("_ScanLineJitter", new Vector2(intensity, intensity));

        var vj = new Vector2(intensity, Time.time % 1);
        _material.SetVector("_VerticalJump", vj);

        _material.SetFloat("_HorizontalShake", intensity * 0.2f);

        var cd = new Vector2(intensity * 0.04f, Time.time * 606.11f);
        _material.SetVector("_ColorDrift", cd);
    }

    public void Update()
    {
        SetIntensity(ManualIntensity);
    }

    public void GlitchFadeInOut(float duration)
    {
        if (activeAnimation == null)
        {
            activeAnimation = StartCoroutine(DoAnimatedGlitch(duration));
        }
    }

    private IEnumerator DoAnimatedGlitch(float duration)
    {
        Debug.Log("Asdf");
        float endTime = Time.time + duration;
        
        // go from min to max intensity and back
        while (Time.time < endTime)
        {
            float startTime = endTime - duration;
            float elapsedTime = Time.time - startTime;
            float normalizedElapsedTime = elapsedTime / duration; // 0-1
            float curved = PlaceholderAnimationCurve.Evaluate(normalizedElapsedTime);
            float t = 1 - 2 * Mathf.Abs(curved-.5f);
            ManualIntensity = t;
            yield return null;
        }

        ManualIntensity = 0;
        activeAnimation = null;
    }
}