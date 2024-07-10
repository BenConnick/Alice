using UnityEngine;

public class SpawnOnDisableBehaviour : MonoBehaviour
{
    public GameObject ObjectToSpawn;
    public bool IgnoreParentActive;

    private void OnDisable()
    {
        if (CanInstantiate())
        {
            GameObject spawned = Instantiate(ObjectToSpawn, transform.parent);
            spawned.transform.localPosition = transform.localPosition;
        }
    }

    private bool CanInstantiate()
    {
        if (!Application.isPlaying)
        {
            return false;
        }

        if (!IgnoreParentActive && !transform.parent.gameObject.activeInHierarchy)
        {
            return false;
        }

        return true;
    }
}
