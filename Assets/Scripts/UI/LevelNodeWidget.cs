using UnityEngine;
using UnityEngine.UI;

public class LevelNodeWidget : MonoBehaviour
{
    [SerializeField] 
    private Image levelCompleteImage;
    
    [SerializeField] 
    private Image levelIconImage;

    [SerializeField] 
    private Sprite levelIcon;
    
    [SerializeField] 
    private Sprite lockIcon;

    public void SetState(bool locked, bool complete)
    {
        if (levelIconImage == null || levelCompleteImage == null) return;
        
        levelCompleteImage.gameObject.SetActive(!complete && !locked);
        levelIconImage.color = complete || locked ? Color.white : Color.black;
        levelIconImage.sprite = locked ? lockIcon : levelIcon;
    }
}
