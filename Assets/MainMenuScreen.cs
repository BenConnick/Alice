using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Sprite[] levelSprites;
    public Image bg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        bg.sprite = levelSprites[(int)GM.CurrentLevel];
    }

    // Update is called once per frame
    void Update()
    {
        // debug stuff
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            GM.OnDebugEvent(GM.DebugEvent.ShowNameEntryScreen);
        }
    }
}
