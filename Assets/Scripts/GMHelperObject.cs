using UnityEngine;

public class GMHelperObject : MonoBehaviour
{
    //public PlatformerAvatar PlatformerAvatar;
    //public PlatformManager PlatformManager;
    //public CameraController CameraController;
    //public WalkaboutUI LabUI;
    public GM.IGameplayUI GameplayUI;
    public GameObject MainMenu;

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
