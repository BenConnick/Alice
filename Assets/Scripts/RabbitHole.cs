﻿using System.Collections.Generic;
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
    public RabbitHoleDisplay OwnerLink { get; set; }
    private float totalFallDistance;
    public float TotalFallDistance => totalFallDistance;
    private float initialHeight;
    private readonly List<LevelChunk> activeChunks = new List<LevelChunk>();
    private readonly List<LevelCollider> activeObstacles = new List<LevelCollider>();
    private ChunkSpawner chunkSpawner;
    private float chunkCursor;

    // per viewport values
    private int vpLives;
    private int vpScore;

    private void Awake()
    {
        chunkSpawner = new ChunkSpawner(chunkPrefabs);
        initialHeight = transform.localPosition.y;
        Time.timeScale = 1f;
    }

    public void Reset()
    {
        vpLives = 99;//GM.MAX_LIVES;

        chunkCursor = -LevelChunk.height;

        // reset level height
        transform.localPosition = new Vector3(transform.localPosition.x, initialHeight, 0);

        // clean up game objects
        foreach (var chunk in activeChunks)
        {
            if (chunk != null) Destroy(chunk.gameObject);
        }

        // clear queue
        activeChunks.Clear();
        activeObstacles.Clear();
    }

    // runs every tick
    private void Update()
    {
        if (!GM.IsGameplayPaused)
        {
            var player = GM.FindSingle<Alice>();
            bool hasFocus = player?.laneContext?.ObstacleContext == this;

            transform.position += new Vector3(0, Time.deltaTime * fallSpeed, 0);
            totalFallDistance = transform.position.y - initialHeight;

            // update active obstacles
            for (int i = activeObstacles.Count-1; i >= 0; i--)
            {
                if (activeObstacles[i] == null)
                    activeObstacles.RemoveAt(i);
            }

            // check collisions
            if (!player.IsFlashing() && hasFocus)
            {
                foreach (var obstacle in activeObstacles)
                {
                    if (CheckOverlap(player, obstacle))
                    {
                        HandleObstacleCollision(player, obstacle);
                    }
                }
            }

            // spawn new obstacles
            PerFrameVariableWatches.SetDebugQuantity("temp", (initialHeight - transform.position.y).ToString() + " < " + (chunkCursor - LevelChunk.height).ToString());
            if (initialHeight - transform.position.y < chunkCursor + 6)
            {
                var newChunkPrefab = chunkSpawner.Force();
                LevelChunk newChunk = Instantiate(newChunkPrefab, transform);
                chunkCursor -= LevelChunk.height;
                newChunk.transform.localPosition = new Vector3(0, chunkCursor - initialHeight, 0);
                activeChunks.Add(newChunk);
                activeObstacles.AddRange(newChunk.Obstacles);

                const int maxChunks = 3;
                if (activeChunks.Count > maxChunks)
                {
                    var oldest = activeChunks[0];
                    activeChunks.RemoveAt(0);
                    Destroy(oldest.gameObject);
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

            if (hasFocus && totalFallDistance > GM.GetLevelLength(GM.CurrentLevel))
            {
                GM.OnGameEvent(GM.NavigationEvent.PlatformerLevelUp);
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
        scoreLabel.text = "<mspace=0.5em>SCORE: " + vpScore.ToString("D4")+"</mspace>";
        // lives
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < vpLives);
        }
    }

    private static bool CheckOverlap(Alice player, LevelCollider levelCollider)
    {
        return levelCollider.OverlapPoint(player.transform.position);
    }

    private void HandleObstacleCollision(Alice player, LevelCollider obstacle)
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