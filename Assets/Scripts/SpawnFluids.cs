using UnityEngine;

public class SpawnFluids : MonoBehaviour
{
    public float Rate = 0.5f;
    public float Scale = 1f;
    public float ColliderRadius = 1f;
    public Sprite circleSprite;
    public Transform spawnLocation;
    public float Variance = 0.1f;

    private float prevSpawnTime;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.time - prevSpawnTime > Rate)
        {
            prevSpawnTime = Time.time;
            var circle = new GameObject();
            circle.transform.SetParent(transform);
            circle.AddComponent<SpriteRenderer>().sprite = circleSprite;
            circle.AddComponent<Rigidbody2D>();
            circle.AddComponent<CircleCollider2D>().radius = ColliderRadius;
            circle.transform.localScale = Vector3.one * Scale;
            circle.transform.position = spawnLocation.position + (Vector3)Random.insideUnitCircle * Variance;
            //circle.AddComponent<DestroyAfterTimeBehavior>().SecondsUntilDestruction = 3f;
            circle.layer = LayerMask.NameToLayer("Fluid");
        }
    }
}
