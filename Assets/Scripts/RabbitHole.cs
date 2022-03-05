using System.Collections.Generic;
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
    private readonly List<LaneEntity> activeObstacles = new List<LaneEntity>();

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

            // update active obstacles
            for (int i = activeObstacles.Count-1; i >= 0; i--)
            {
                if (activeObstacles[i] == null)
                    activeObstacles.RemoveAt(i);
            }

            // check collisions
            var player = GM.FindComp<LaneCharacterMovement>();
            foreach (var obstacle in activeObstacles)
            {
                if (LaneUtils.CheckOverlap(player, obstacle))
                {
                    // shake, flash, subtract lives
                    GM.FindComp<GameplayCameraBehavior>().Shake();
                    player.StartFlashing();
                    GM.Lives--;

                    // flash the collider
                    var flashing = obstacle.gameObject.AddComponent<FlashingBehavior>();
                    flashing.flashOffTime = 0.08f;
                    flashing.StartFlashing();

                    // bump up the removal time (if applicable)
                    var destroyer = obstacle.gameObject.GetComponent<DestroyAfterTimeBehavior>();
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

public static class LaneUtils
{
    public static int NumLanes = 10;
    public static float LaneScale = 0.6f;

    public static float GetWorldPosition(LaneEntity entity)
    {
        return (entity.Lane - NumLanes * .5f + entity.WidthLanes * .5f) * LaneScale;
    }

    public static bool CheckOverlap(LaneEntity a, LaneEntity b)
    {
        // lane
        if (a.Lane + a.WidthLanes <= b.Lane) return false; // A left of B
        if (b.Lane + b.WidthLanes <= a.Lane) return false; // B left of A

        // height
        if (a.Y - a.Height * .5f > b.Y) return false; // A above B
        if (a.Y + a.Height * .5f < b.Y) return false; // A below B

        // must be overlapping
        return true;
    }
}