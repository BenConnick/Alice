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

    // per viewport values
    private int vpLives;
    private int vpScore;

    private void Awake()
    {
        currentSpawner = new ChunkSpawner(chunkPrefabs);
        initialHeight = transform.localPosition.y;
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
    }

    public void Reset()
    {
        vpLives = 99;//GM.MAX_LIVES;

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
            if (!player.IsFlashing() && player?.laneContext?.ObstacleContext == this)
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
                var gameCam = GM.FindSingle<GameplayInnerDisplayCamera>().GetComponent<Camera>();
                var yPos = -gameCam.orthographicSize * 2; // below bottom of the screen
                LevelChunk newChunk = Instantiate(newChunkPrefab, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity, transform);
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
        vpScore = Mathf.FloorToInt(progressTotal); // <- putting the actual score in the UI rendering is questionable at best...
        scoreLabel.text = "SCORE:<br>"+vpScore;
        // lives
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < vpLives);
        }
    }

    private void HandleObstacleCollision(Alice player, LaneEntity obstacle)
    {
        if (obstacle.HasTag(LaneEntity.Tag_DamageOnHit))
        {
            // flash the collider
            var flashing = obstacle.gameObject.AddComponent<FlashingBehavior>();
            flashing.flashOffTime = 0.08f;
            flashing.StartFlashing();

            // bump up the removal time (if applicable)
            var destroyer = obstacle.gameObject.GetComponent<DestroyAfterTimeBehavior>();
            if (destroyer != null) destroyer.SecondsUntilDestruction = Mathf.Min(destroyer.SecondsUntilDestruction, 2);

            // shake, flash, subtract lives
            // GM.FindSingle<GameplayCameraBehavior>().Shake(); DISABLED FOR EDITING
            player.StartFlashing();
            //SubtractLife();
        }
        if (obstacle.HasTag(LaneEntity.Tag_GrowOnHit))
        {
            player.OnGrow();
        }
        if (obstacle.HasTag(LaneEntity.Tag_ShrinkOnHit))
        {
            player.OnShrink();
        }
    }

    private void SubtractLife()
    {
        vpLives--;
        if (vpLives <= 0)
        {
            GM.OnGameEvent(GM.NavigationEvent.PlatformerGameOver);
        }
    }
}