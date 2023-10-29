using UnityEngine;

/// <summary>
/// Purpose: First MonoBehavior
/// Initializes and Updates the global Game Manager
/// Stores references to scene objects
/// </summary>
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
    public Transform[] SearchRoots;
    public float[] LevelLengths;
    public float DefaultFallSpeed = 6;
    public float FallAcceleration;
    public AnimationCurve FallSpeedVariation;

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
