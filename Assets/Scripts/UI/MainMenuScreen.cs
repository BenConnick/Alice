using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Sprite[] levelSprites;
    public Image bg;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        //bg.sprite = levelSprites[(int)GM.CurrentMenuStage];
        animator.SetTrigger("ResetAnimationTrigger");
    }

    // Update is called once per frame
    void Update()
    {
        // debug stuff
        if (Input.GetKeyUp(KeyCode.Alpha0)) SetSprite(0);
        if (Input.GetKeyUp(KeyCode.Alpha1)) SetSprite(1);
        if (Input.GetKeyUp(KeyCode.Alpha2)) SetSprite(2);
        if (Input.GetKeyUp(KeyCode.Alpha3)) SetSprite(3);
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            ApplicationLifetime.OnDebugEvent(ApplicationLifetime.DebugEvent.ShowNameEntryScreen);
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            ApplicationLifetime.OnDebugEvent(ApplicationLifetime.DebugEvent.SetLevelCaterpillar);
        }
    }

    private void SetSprite(int i)
    {
        bg.sprite = levelSprites[i];
    }

    public void OnStartPressed()
    {
        GameEventHandler.OnGameEvent(NavigationEvent.MainMenuGoNext);
    }
}
