using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayScreenBehavior : MonoBehaviour
{
    public CanvasGroup StoryGroup;
    public CanvasGroup GameplayGroup;
    public StoryLabel StoryController;

    public void ShowStory(string passage)
    {
        ContextualInputSystem.UICapturedInput = true;
        StartCoroutine(CrossFadeText(true));
        StoryController.ShowStory(passage);
    }

    public void ShowGame()
    {
        ContextualInputSystem.UICapturedInput = false;
        StartCoroutine(CrossFadeText(false));
    }

    private IEnumerator CrossFadeText(bool showText, float duration = .6f)
    {
        CanvasGroup fadeOutGroup = showText ? GameplayGroup : StoryGroup;
        CanvasGroup fadeInGroup = showText ? StoryGroup : GameplayGroup;

        // instantly swap which group recieves input
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
}
