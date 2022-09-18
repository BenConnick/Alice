using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private GameObject[] heartIcons;
    [SerializeField] private RectTransform progressMarker;
    [SerializeField] private UnityEngine.UI.Button startButton;

    [Header("Config")]
    public float fallSpeed;
    public ObstacleSpawnersConfig SpawnersConfig;

    [Header("Animation")]
    public float introAnimationDistance;
    public float introAnimationSpeed;
    public MenuGraphics menuGraphics;

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
    private int vpLives;
    private int vpScore;

    private void Awake()
    {
        chunkSpawner = new ChunkSpawner(chunkPrefabs);
        initialHeight = transform.localPosition.y;
        Time.timeScale = 1f;
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
    }

    public void Reset()
    {
        chunkSpawner = new ChunkSpawner(chunkPrefabs);

        vpLives = 99;//GM.MAX_LIVES;

        chunkCursor = -LevelChunk.height - introAnimationDistance;

        // reset level height
        transform.localPosition = new Vector3(transform.localPosition.x, initialHeight, transform.localPosition.z);

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
        return transform.localPosition.y - (initialHeight+introAnimationDistance);
    }

    // runs every tick
    private void Update()
    {
        if (mode == AnimationMode.Intro)
        {
            float normalizedIntroDistance = Mathf.Clamp01(transform.localPosition.y - initialHeight) / (introAnimationDistance * 0.01f);
            transform.position += new Vector3(0, Time.deltaTime * fallSpeed * Mathf.Lerp(0.05f, 1, normalizedIntroDistance));
            //transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, initialHeight + introAnimationDistance + 1f, Time.deltaTime * introAnimationSpeed), transform.localPosition.z);
            if (transform.localPosition.y > initialHeight + introAnimationDistance)
            {
                OnIntroComplete();
            }
        }
        else if (mode == AnimationMode.Outro)
        {
            transform.position += new Vector3(0, Time.deltaTime * fallSpeed, 0);
            //transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, outroStartHeight + introAnimationDistance + 1f, Time.deltaTime * introAnimationSpeed), transform.localPosition.z);
            if (transform.localPosition.y > outroStartHeight + introAnimationDistance)
            {
                OnOutroComplete();
            }
        }

        if (!GM.IsGameplayPaused)
        {
            var player = GM.FindSingle<Alice>();
            bool hasFocus = player.laneContext?.ObstacleContext == this;

            if (mode == AnimationMode.Interactive)
            {
                transform.position += new Vector3(0, Time.deltaTime * fallSpeed, 0);
            }
            totalFallDistance = transform.position.y - (initialHeight+introAnimationDistance);

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
            if (initialHeight - transform.position.y < chunkCursor + 6
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
        scoreLabel.text = "" + vpScore.ToString("D3")+Util.CurrencyChar;
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
            flashing.flashOffTime = 0.07f;
            flashing.StartFlashing();

            // bump up the removal time (if applicable)
            var destroyer = obstacle.gameObject.GetComponent<DestroyAfterTimeBehavior>();
            if (destroyer != null) destroyer.SecondsUntilDestruction = Mathf.Min(destroyer.SecondsUntilDestruction, 2);

            // shake, flash, subtract lives
            TimeDistortionController.PlayImpactFrame();
            GM.FindSingle<GameplayCameraBehavior>().Shake(); // DISABLED FOR EDITING
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