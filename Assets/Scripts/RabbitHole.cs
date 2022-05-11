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
    public float fallSpeed;
    public ObstacleSpawnersConfig SpawnersConfig;

    [Header("Assets")]
    [SerializeField] private LevelChunk[] chunkPrefabs;
    [SerializeField] private GameObject[] obstaclePrefabs;

    // fields
    private float totalFallDistance;
    public float TotalFallDistance => totalFallDistance;
    private float initialHeight;
    private readonly List<LaneEntity> activeObstacles = new List<LaneEntity>();
    private ChunkSpawner currentSpawner;

    private void Awake()
    {
        currentSpawner = new ChunkSpawner(chunkPrefabs);
        initialHeight = transform.localPosition.y;
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
    }

    public void Reset()
    {
        // reset level height
        transform.localPosition = new Vector3(transform.localPosition.x, initialHeight, 0);

        // clean up game objects
        foreach (var ob in activeObstacles)
        {
            if (ob != null && ob.gameObject != null)
            {
                // destroy chunk
                var chunk = ob.GetComponentInParent<LevelChunk>();
                if (chunk != null && chunk.gameObject != null) Destroy(chunk.gameObject);
                // fallback on individual obstacle destroy
                else Destroy(ob.gameObject);
            }
        }

        // clear queue
        activeObstacles.Clear();
    }

    // runs every tick
    private void Update()
    {
        if (!GM.IsGameplayPaused)
        {
            transform.position += new Vector3(0, Time.deltaTime * fallSpeed, 0);
            totalFallDistance = transform.position.y;

            // update active obstacles
            for (int i = activeObstacles.Count-1; i >= 0; i--)
            {
                if (activeObstacles[i] == null)
                    activeObstacles.RemoveAt(i);
            }

            // check collisions
            var player = GM.FindSingle<Alice>();
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
            var newChunkPrefab = currentSpawner.Update(Time.deltaTime);
            if (newChunkPrefab != null)
            {
                var gameCam = GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>();
                var yPos = -gameCam.orthographicSize * 2; // below bottom of the screen
                LevelChunk newChunk = Instantiate(newChunkPrefab, new Vector3(0, yPos, 0), Quaternion.identity, transform);
                for (int i = 0; i < newChunk.Obstacles.Length; i++)
                {
                    // spawn
                    var entity = newChunk.Obstacles[i];
                    activeObstacles.Add(entity);
                }
            }

            // update UI
            UpdateGameplayUI();

            // debug
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // shrink
                GM.FindSingle<Alice>().OnShrink();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // grow
                GM.FindSingle<Alice>().OnGrow();
            }
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