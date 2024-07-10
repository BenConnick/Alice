using UnityEngine;

public class SpawnOnDestroyBehaviour : MonoBehaviour
{
    public GameObject ObjectToSpawn;

    private void OnDestroy()
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

        return true;
    }
}
