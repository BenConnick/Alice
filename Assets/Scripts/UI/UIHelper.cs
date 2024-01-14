using System.Collections;
using UnityEngine;

public static class UIHelper
{
    public static IEnumerator CrossFadeCanvasGroups(CanvasGroup fadeOutGroup, CanvasGroup fadeInGroup, float duration = .6f)
    {
        // instantly swap which group receives input
        fadeInGroup.interactable = true;
        fadeInGroup.blocksRaycasts = true;
        fadeOutGroup.interactable = false;
        fadeOutGroup.blocksRaycasts = false;

        // cross-fade
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < duration)
        {
            float t = (Time.unscaledTime - startTime) / duration;
            fadeInGroup.alpha = t;
            fadeOutGroup.alpha = 1-t;
            yield return null;
        }
        // set exact values to finish
        fadeOutGroup.alpha = 0;
        fadeInGroup.alpha = 1;
    }

    public static void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        // instantly swap which group receives input
        group.interactable = active;
        group.blocksRaycasts = active;
        
        // instantly change which group is visible
        group.alpha = active ? 1 : 0;
    }
}
