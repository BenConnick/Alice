using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] 
    private LevelNodeWidget[] nodeWidgets;

    public void UpdateUI()
    {
        int maxLevel = GameplayManager.ToLevelIndex(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
        for (int i = 0; i < nodeWidgets.Length; i++)
        {
            bool isLocked = i > maxLevel;
            bool isComplete = i < maxLevel;
            nodeWidgets[i].SetState(isLocked, isComplete);
        }
    }
}
