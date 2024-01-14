using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class GameplayScreenBehavior : MonoBehaviour
{
    public CanvasGroup StoryGroup;
    public CanvasGroup GameplayGroup;
    public CanvasGroup LevelSelectGroup;
    public MovieCardLabel StoryController;

    public void ShowStory()
    {
        ContextualInputSystem.UICapturedInput = true;
        UIHelper.SetCanvasGroupActive(StoryGroup, true);
    }

    public void HideStory()
    {
        UIHelper.SetCanvasGroupActive(StoryGroup, false);
    }

    public void ShowGame()
    {
        ContextualInputSystem.UICapturedInput = false;
        UIHelper.SetCanvasGroupActive(GameplayGroup, true);
    }

    public void HideGame()
    {
        UIHelper.SetCanvasGroupActive(GameplayGroup, false);
    }

    public void ShowLevelSelect()
    {
        ContextualInputSystem.UICapturedInput = true;
        UIHelper.SetCanvasGroupActive(LevelSelectGroup, true);
    }

    public void HideLevelSelect()
    {
        UIHelper.SetCanvasGroupActive(LevelSelectGroup, false);
    }

    public void OnLevelButtonPressed(int levelIndex)
    {
        Debug.Log($"Level button {levelIndex} pressed");
    }
}
