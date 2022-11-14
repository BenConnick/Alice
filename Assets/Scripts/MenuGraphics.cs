using UnityEngine;

public class MenuGraphics : MonoBehaviour
{
    public GameObject[] PerStageArt;

    public void ShowStageArt(LevelType lvl)
    {
        int stageIndex = (int)lvl;
        for (int i = 0; i < PerStageArt.Length; i++)
        {
            PerStageArt[i].SetActive(i == stageIndex);
        }
    }

    public void HideAllStageArt()
    {
        for (int i = 0; i < PerStageArt.Length; i++)
        {
            PerStageArt[i].SetActive(false);
        }
    }
}
