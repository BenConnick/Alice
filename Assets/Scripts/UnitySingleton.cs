using UnityEngine;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("This singleton was instantiated multiple times");
            return;
        }
        Instance = this as T;
    }
}
