using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Purpose: First MonoBehavior
/// Initializes and Updates the global Game Manager
/// Stores references to scene objects
/// </summary>
public class BootstrapObject : MonoBehaviour
{
    public Transform[] SearchRoots;

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
