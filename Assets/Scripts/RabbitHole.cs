using System.Collections.Generic;
using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    [Header("Config")]
    public float fallSpeed;
    public ObstacleSpawnersConfig SpawnersConfig;

    [Header("Animation")]
    public float introAnimationDistance;
    public float introAnimationSpeed;
    public MenuGraphics menuGraphics;

    public float OutroAnimationDistance => introAnimationDistance;

    [Header("Assets")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private LevelChunk[] rabbitChunkPrefabs;
    [SerializeField] private LevelChunk[] caterpillarChunkPrefabs;
    [SerializeField] private LevelChunk[] chessChunkPrefabs;
    [SerializeField] private LevelChunk[] teaPartyChunkPrefabs;
    [SerializeField] private LevelChunk[] queenChunkPrefabs;

    // fields
    public RabbitHoleDisplay OwnerLink { get; set; }
    private float totalFallDistance;
    public float TotalFallDistance => totalFallDistance;
    private float initialHeight;
    public float InitialHeight => initialHeight;
    private float outroStartHeight;
    private readonly List<LevelChunk> activeChunks = new List<LevelChunk>();
    private readonly List<LevelCollider> activeObstacles = new List<LevelCollider>();
    private ChunkSpawner chunkSpawner;
    private float chunkCursor;
    private AnimationMode mode;

    public enum AnimationMode
    {
        Default,
        Interactive,
        Intro,
        Outro // no player control
    }

    private LevelChunk[] chunkPrefabs
    {
        get
        {
            switch (GM.CurrentLevel)
            {
                case LevelType.Default:
                    break;
                case LevelType.RabbitHole:
                    return rabbitChunkPrefabs;
                case LevelType.Caterpillar:
                    return caterpillarChunkPrefabs;
                case LevelType.CheshireCat:
                    return chessChunkPrefabs;
                case LevelType.MadHatter:
                    return teaPartyChunkPrefabs;
                case LevelType.QueenOfHearts:
                    return queenChunkPrefabs;
                default:
                    break;
            }
            return rabbitChunkPrefabs;
        }
    }

    // per viewport values
    public int VpLives { get; set; }
    public int VpScore { get; set; }

    private void Awake()
    {
        chunkSpawner = new ChunkSpawner(chunkPrefabs);
        initialHeight = transform.localPosition.y;
        Time.timeScale = 1f;
    }

    // runs every tick
    private void Update()
    {
        if (mode == AnimationMode.Intro)
        {
            float normalizedIntroDistance = Mathf.Clamp01(transform.localPosition.y - initialHeight) / (introAnimationDistance * 0.01f);
            transform.localPosition += new Vector3(0, Time.deltaTime * fallSpeed * Mathf.Lerp(0.05f, 1, normalizedIntroDistance));
            if (transform.localPosition.y > initialHeight + introAnimationDistance)
            {
                OnIntroComplete();
            }
        }
        else if (mode == AnimationMode.Outro)
        {
            transform.localPosition += new Vector3(0, Time.deltaTime * fallSpeed, 0);

            // alice lerp to resting pos
            {
                float t = (transform.localPosition.y - outroStartHeight) / OutroAnimationDistance;
                Vector3 characterTargetRestingPos = new Vector3(0, -2, 0);
                Transform aliceTransform = GM.FindSingle<Alice>().transform;
                aliceTransform.position = Vector3.Lerp(aliceTransform.position, characterTargetRestingPos, t);
            }

            if (transform.localPosition.y > outroStartHeight + OutroAnimationDistance)
            {
                OnOutroComplete();
            }
        }

        if (!GM.IsGameplayPaused)
        {
            var player = GM.FindSingle<Alice>();
            bool hasFocus = player.movementContext?.ObstacleContext == this;

            if (mode == AnimationMode.Interactive)
            {
                transform.localPosition += new Vector3(0, Time.deltaTime * fallSpeed, 0);
            }
            totalFallDistance = transform.localPosition.y - (initialHeight+introAnimationDistance);

            // update active obstacles
            for (int i = activeObstacles.Count-1; i >= 0; i--)
            {
                if (activeObstacles[i] == null)
                    activeObstacles.RemoveAt(i);
            }

            // check collisions
            if (hasFocus)
            {
                foreach (var obstacle in activeObstacles)
                {
                    bool ignoresInvincibility = obstacle.HasTag(LevelCollider.Tag_MoneyOnHit);
                    bool invincible = player.IsFlashing();
                    if (ignoresInvincibility || !invincible)
                    {
                        if (player.CheckOverlap(obstacle))
                        {
                            player.HandleObstacleCollision(obstacle);
                        }
                    }
                }
            }

            // spawn new obstacles
            PerFrameVariableWatches.SetDebugQuantity("temp", (initialHeight - transform.localPosition.y).ToString() + " < " + (chunkCursor - LevelChunk.height).ToString());
            if (initialHeight - transform.localPosition.y < chunkCursor + 6
                // stop spawning chunks before the level end
                && totalFallDistance < GM.GetLevelLength(GM.CurrentLevel) - LevelChunk.height * 2)
            {
                var newChunkPrefab = chunkSpawner.Force();
                LevelChunk newChunk = Instantiate(newChunkPrefab, transform);
                chunkCursor -= Mathf.Sign(fallSpeed) * LevelChunk.height;
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

            if (hasFocus && mode == AnimationMode.Interactive && totalFallDistance > GM.GetLevelLength(GM.CurrentLevel))
            {
                GM.OnGameEvent(GM.NavigationEvent.PlatformerLevelUp);
            }
        }
    }



    public void PlayIntroAnimation()
    {
        menuGraphics.ShowStageArt(GM.CurrentLevel);
        mode = AnimationMode.Intro;
        //OwnerLink?.Overlay?.SetActive(false);
    }

    private void OnIntroComplete()
    {
        OwnerLink.Overlay?.SetActive(true);
        mode = AnimationMode.Interactive;
    }

    public void PlayOutroAnimation()
    {
        outroStartHeight = transform.localPosition.y;
        mode = AnimationMode.Outro;
        //OwnerLink?.Overlay?.SetActive(false);
        menuGraphics.transform.localPosition = new Vector3(0, -outroStartHeight - introAnimationDistance, 0);
        menuGraphics.ShowStageArt(GM.CurrentLevel);
    }

    private void OnOutroComplete()
    {
        OwnerLink.Overlay?.SetActive(false);
        mode = AnimationMode.Default;
        GM.FindSingle<Alice>().BecomeButton();
        menuGraphics.transform.localPosition = new Vector3(0, -initialHeight, 0);
        Reset();
        GM.FindSingle<GameplayScreenBehavior>().ShowStory(GM.PassageStartPrefix);
    }

    public void Reset()
    {
        chunkSpawner = new ChunkSpawner(chunkPrefabs);

        VpLives = GM.MAX_LIVES;

        chunkCursor = -LevelChunk.height - introAnimationDistance;

        // reset level height
        transform.localPosition = new Vector3(transform.localPosition.x, initialHeight, transform.localPosition.z);

        // clean up game objects
        RemoveAllChunks();
    }

    private void RemoveAllChunks()
    {
        // clean up game objects
        foreach (var chunk in activeChunks)
        {
            if (chunk != null) Destroy(chunk.gameObject);
        }

        // clear queue
        activeChunks.Clear();
        activeObstacles.Clear();
    }

    public float GetIntroOffset()
    {
        return transform.localPosition.y - (initialHeight + introAnimationDistance);
    }
}