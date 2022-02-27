using StableFluids;
using TMPro;
using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private GameObject[] heartIcons;
    [SerializeField] private RectTransform progressMarker;

    [Header("Config")]
    [SerializeField] private float secondsBetweenObstacles;
    [SerializeField] private float obstacleXMax;
    [SerializeField] private float fallSpeed;

    [Header("Assets")]
    [SerializeField] private GameObject[] obstaclePrefabs;

    // fields
    private float lastObstableSpawnTime;
    private float totalFallDistance;
    public float TotalFallDistance => totalFallDistance;
    private GameObject[] shuffledObstacleQueue = new GameObject[0];
    private int shuffledObstacleIndex = int.MaxValue;
    private float initialHeight;

    private void Awake()
    {
        initialHeight = transform.localPosition.y;
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
    }

    // runs every tick
    private void Update()
    {
        if (!GM.IsGameplayPaused)
        {
            Vector3 movement;
            movement = new Vector3(0, fallSpeed * SmokeRendering.FixedTimeInterval, 0);
            transform.position += movement;
            GM.FindComp<SmokeRendering>().DriveWithGameplay();

            totalFallDistance = transform.position.y;

            // check collisions
            var player = GM.FindComp<CharacterController>();
            var results = new Collider2D[100];
            var filter = new ContactFilter2D().NoFilter();
            filter.SetLayerMask(LayerMask.NameToLayer("Obstacle"));
            int numCollisions = player.GetComponent<Collider2D>().OverlapCollider(filter, results);
            if (numCollisions > 0)
            {
                // log
                //Debug.Log("Collision! " + results[0].name);

                // player FX
                // shake, flash, subtract lives
                GM.FindComp<GameplayCameraBehavior>().Shake();
                player.StartFlashing();
                GM.Lives--;

                for (int i = 0; i < numCollisions; i++)
                {
                    // make the collider react to being hit
                    // bounce away from the player and rotate
                    Collider2D collider = results[i];
                    var collision = collider.gameObject;
                    Vector3 colliderCenter = collision.transform.position;
                    Vector3 playerCenter = player.transform.position;
                    //Vector3 closestPoint = collider.ClosestPoint(player.transform.position);
                    Vector3 toVec = Vector3.Normalize(colliderCenter - playerCenter);
                    float torquePower = Mathf.Abs(Mathf.Abs(toVec.x) - Mathf.Abs(toVec.y)); // 0 to 1
                    float dir = ((playerCenter.y > colliderCenter.y) ^ (playerCenter.x > colliderCenter.x) ? 1 : -1);
                    float torque = 100*torquePower * dir;
                    var rbody = collision.GetOrAddComponent<Rigidbody2D>();
                    rbody.gravityScale = 0;
                    rbody.AddTorque(torque);
                    rbody.drag = 1.2f;
                    rbody.angularDrag = 0.2f;
                    rbody.AddForceAtPosition(toVec * (10 / (2 + 2 * torquePower)), playerCenter, ForceMode2D.Impulse);

                    // also push back the player for added feedback
                    player.PauseInput(0.1f);
                    player.Push(-toVec);

                    // remove the collider to prevent repeat hits
                    Destroy(collider);

                    // flash the collider
                    var flashing = collision.AddComponent<FlashingBehavior>();
                    flashing.flashOffTime = 0.08f;
                    flashing.StartFlashing();

                    // remove after a few seconds
                    var destroyer = collision.GetComponent<DestroyAfterTimeBehavior>();
                    if (destroyer != null) destroyer.SecondsUntilDestruction = Mathf.Min(destroyer.SecondsUntilDestruction, 2);
                }
            }

            // spawn new obstacles
            if (Time.time - lastObstableSpawnTime > secondsBetweenObstacles)
            {
                lastObstableSpawnTime = Time.time;

                // spawn random obstacle
                if (shuffledObstacleIndex >= shuffledObstacleQueue.Length)
                {
                    shuffledObstacleQueue = new GameObject[obstaclePrefabs.Length];
                    obstaclePrefabs.CopyTo(shuffledObstacleQueue, 0);
                    Util.Shuffle(shuffledObstacleQueue);
                    shuffledObstacleIndex = 0;
                }
                var prefab = shuffledObstacleQueue[shuffledObstacleIndex];
                shuffledObstacleIndex++;
                var gameCam = GM.FindComp<GameplayCameraBehavior>().GetComponent<Camera>();
                var yPos = -gameCam.orthographicSize * 2; // below bottom of the screen
                var xPos = Random.Range(-obstacleXMax, obstacleXMax); // at a random position
                var inst = GameObject.Instantiate(prefab, new Vector3(xPos, yPos, 0), Quaternion.identity, transform);
            }

            // update UI
            float progressTotal = transform.localPosition.y - initialHeight;
            float progressPercent = progressTotal / GetLength(GM.LevelType);
            // score
            scoreLabel.text = "SCORE:<br>"+Mathf.FloorToInt(progressTotal);
            // lives
            for (int i = 0; i < heartIcons.Length; i++)
            {
                heartIcons[i].SetActive(i < GM.Lives);
            }
            // progress
            progressMarker.anchorMax = progressMarker.anchorMin = new Vector2(0.5f, 1-progressPercent);
            progressMarker.anchoredPosition = Vector2.zero;
        }
    }

    private static float GetLength(LevelType level)
    {
        switch (level)
        {
            case LevelType.Default:
                return 999; // TODO
            case LevelType.RabbitHole:
                return 150;
            case LevelType.Caterpillar:
                return 150;
            case LevelType.CheshireCat:
                return 150;
            case LevelType.MadHatter:
                return 150;
            case LevelType.TweedleDum:
                return 150;
            case LevelType.QueenOfHearts:
                return 150;
            default:
                return 999;
        }
    }
}
