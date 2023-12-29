using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class GameplayScreenBehavior : MonoBehaviour
{
    public CanvasGroup StoryGroup;
    public CanvasGroup GameplayGroup;
    public MovieCardLabel StoryController;

    public void ShowStory(string passage)
    {
        ContextualInputSystem.UICapturedInput = true;
        UIHelper.SwapActiveCanvasGroup(StoryGroup, GameplayGroup);
    }

    public void ShowGame()
    {
        ContextualInputSystem.UICapturedInput = false;
        UIHelper.SwapActiveCanvasGroup(GameplayGroup,StoryGroup);
    }
}
