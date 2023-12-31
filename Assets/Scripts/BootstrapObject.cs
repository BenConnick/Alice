using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Purpose: First MonoBehavior
/// Initializes and Updates the global Game Manager
/// Stores references to scene objects
/// </summary>
public class BootstrapObject : MonoBehaviour
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
    public AnimationCurve FallSpeedVariation;
    [FormerlySerializedAs("GameConfigsAsset")] public MasterConfigAsset MasterConfigsAsset;

    [Header("MenuSprites")]
    public Sprite[] MenuSprites;

    // Start is called before the first frame update
    private void Awake()
    {
        ApplicationLifetime.Init(this);
    }

    private void Update()
    {
        ApplicationLifetime.Tick();
    }

    private void LateUpdate()
    {
        ApplicationLifetime.LateTick();
    }
}
