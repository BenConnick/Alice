using UnityEngine;

public class MenuGraphics : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] PerStageArt;

    public GameObject ShowingArt;
    private int showingIndex = -1;

    public void ShowStageArt(LevelType lvl)
    {
        if (ShowingArt != null)
        {
            HideAllStageArt();
        }

        int stageIndex = (int)lvl;
        for (int i = 0; i < PerStageArt.Length; i++)
        {
            if (i == stageIndex && i != showingIndex)
                ShowingArt = Instantiate(PerStageArt[i], transform);
        }
    }

    public void HideAllStageArt()
    {
        if (ShowingArt != null)
        {
            Destroy(ShowingArt);
            ShowingArt = null;
            showingIndex = -1;
        }
    }
}
