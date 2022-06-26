using UnityEngine;

public class GMHelperObject : MonoBehaviour
{
    //public PlatformerAvatar PlatformerAvatar;
    //public PlatformManager PlatformManager;
    //public CameraController CameraController;
    //public WalkaboutUI LabUI;
    [Header("Screens")]
    public GameObject MainMenu;
    public GameObject Gameplay;
    public GameObject ScoreBoard;
    public GameObject EnterNameScreen;

    [Header("MenuSprites")]
    public Sprite[] MenuSprites;

    // Start is called before the first frame update
    private void Awake()
    {
        GM.Init(this);
    }

    private void Update()
    {
        GM.Tick();
    }

    private void LateUpdate()
    {
        GM.LateTick();
    }
}
