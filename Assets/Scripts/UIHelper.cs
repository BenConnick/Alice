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
    
    public static void SwapActiveCanvasGroup(CanvasGroup activeGroup, CanvasGroup previousGroup)
    {
        // instantly swap which group receives input
        activeGroup.interactable = true;
        activeGroup.blocksRaycasts = true;
        previousGroup.interactable = false;
        previousGroup.blocksRaycasts = false;
        
        // instantly change which group is visible
        previousGroup.alpha = 0;
        activeGroup.alpha = 1;
    }
}
