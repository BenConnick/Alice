using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    public CanvasGroup StoryGroup;
    public CanvasGroup GameplayGroup;
    public CanvasGroup LevelSelectGroup;
    public MovieCardLabel StoryController;
    public TextMeshProUGUI ToastLabel;

    public void ShowStory()
    {
        ContextualInputSystem.UICapturedInput = true;
        UIHelper.SetCanvasGroupActive(StoryGroup, true);
    }

    public void HideStory()
    {
        UIHelper.SetCanvasGroupActive(StoryGroup, false);
    }

    public void SetGameViewVisible()
    {
        ContextualInputSystem.UICapturedInput = false;
        UIHelper.SetCanvasGroupActive(GameplayGroup, true);
    }

    public void SetGameViewHidden()
    {
        UIHelper.SetCanvasGroupActive(GameplayGroup, false);
    }

    public void ShowLevelSelect()
    {
        ContextualInputSystem.UICapturedInput = true;
        UIHelper.SetCanvasGroupActive(LevelSelectGroup, true);
        World.Get<LevelSelectUI>().UpdateUI();
    }

    public void HideLevelSelect()
    {
        UIHelper.SetCanvasGroupActive(LevelSelectGroup, false);
    }

    public void OnLevelButtonPressed(int levelIndex)
    {
        Debug.Log($"Level button {levelIndex} pressed");
        if (GameplayManager.IsLevelLocked(levelIndex))
        {
            ShowToast("LOCKED");
            return;
        }
        GameplayManager.ChangeSelectedLevel(MasterConfig.Values.LevelConfigs[levelIndex].LevelType);
        GameplayManager.Fire(GlobalGameEvent.LevelSelectionConfirmed);
    }

    public void ReloadAll()
    {
        World.Get<LevelSelectUI>().UpdateUI();
    }

    public void ShowToast(string message, float duration = 2f)
    {
        StartCoroutine(ShowToastForSeconds(message, duration));
    }

    private IEnumerator ShowToastForSeconds(string toast, float seconds)
    {
        ToastLabel.text = toast;
        ToastLabel.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(seconds);
        ToastLabel.gameObject.SetActive(false);
    }
}
