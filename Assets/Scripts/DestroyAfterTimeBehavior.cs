using UnityEngine;

public class DestroyAfterTimeBehavior : MonoBehaviour
{
    public float SecondsUntilDestruction;
    public bool IgnorePausedGameplay;

    // Update is called once per frame
    void Update()
    {
        if (SecondsUntilDestruction <= 0)
        {
            Destroy(gameObject);
            return;
        }
        if (IgnorePausedGameplay || !GM.IsGameplayPaused)
            SecondsUntilDestruction -= Time.deltaTime;
    }
}
