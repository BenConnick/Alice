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
    [SerializeField] private float fallSpeed;
    public ObstacleSpawnersConfig SpawnersConfig;

    [Header("Assets")]
    [SerializeField] private GameObject[] chunkPrefabs;
    [SerializeField] private GameObject[] obstaclePrefabs;

    // fields
    private float lastObstableSpawnTime;
    private float totalFallDistance;
    public float TotalFallDistance => totalFallDistance;
    private float initialHeight;
    private readonly List<LaneEntity> activeObstacles = new List<LaneEntity>();
    private ObstacleSpawner currentSpawner;

    private void Awake()
    {
        currentSpawner = new RabbitHoleObstacleSpawner(obstaclePrefabs);
        initialHeight = transform.localPosition.y;
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
    }

    public void Reset()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, initialHeight, 0);
        foreach (var ob in activeObstacles)
        {
            if (ob != null && ob.gameObject != null) Destroy(ob.gameObject);
        }
        activeObstacles.Clear();
    }

    // runs every tick
    private void Update()
    {
        if (!GM.IsGameplayPaused)
        {
            Vector3 movement;
            movement = new Vector3(0, fallSpeed * SmokeRendering.FixedTimeInterval, 0);
            transform.position += movement;
            GM.FindSingle<SmokeRendering>().DriveWithGameplay();

            totalFallDistance = transform.position.y;

            // update active obstacles
            for (int i = activeObstacles.Count-1; i >= 0; i--)
            {
                if (activeObstacles[i] == null)
                    activeObstacles.RemoveAt(i);
            }

            // check collisions
            var player = GM.FindSingle<LaneCharacterMovement>();
            if (!player.IsFlashing())
            {
                foreach (var obstacle in activeObstacles)
                {
                    if (LaneUtils.CheckOverlap(player, obstacle))
                    {
                        HandleObstacleCollision(player, obstacle);
                    }
                }
            }

            // spawn new obstacles
            var newObstacles = currentSpawner.Update(Time.deltaTime);
            for (int i = 0; i < newObstacles.Length; i++)
            {
                // spawn
                var spawn = newObstacles[i];
                var inst = Instantiate(spawn.Prefab, new Vector3(LaneUtils.GetWorldPosition(spawn.Prefab.GetComponent<LaneEntity>(),spawn.Lane), spawn.YPos, 0), Quaternion.identity, transform);
                var entity = inst.GetComponent<LaneEntity>();
                entity.Lane = spawn.Lane;
                activeObstacles.Add(entity);
            }

            // update UI
            UpdateGameplayUI();
        }
    }

    private void UpdateGameplayUI()
    {
        float progressTotal = transform.localPosition.y - initialHeight;
        //float progressPercent = progressTotal / GetLength(GM.LevelType);
        // progress
        //progressMarker.anchorMax = progressMarker.anchorMin = new Vector2(0.5f, 1 - progressPercent);
        //progressMarker.anchoredPosition = Vector2.zero;
        // score
        GM.CurrentScore = Mathf.FloorToInt(progressTotal); // <- putting the actual score in the UI rendering is questionable at best...
        scoreLabel.text = "SCORE:<br>"+GM.CurrentScore;
        // lives
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < GM.Lives);
        }
    }

    private void HandleObstacleCollision(LaneCharacterMovement player, LaneEntity obstacle)
    {
        // shake, flash, subtract lives
        GM.FindSingle<GameplayCameraBehavior>().Shake();
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