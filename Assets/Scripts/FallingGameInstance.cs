using System.Collections.Generic;
using UnityEngine;

public class FallingGameInstance
{
    public static List<FallingGameInstance> All = new List<FallingGameInstance>();
    public static FallingGameInstance Current => ContextualInputSystem.ActiveGameInstance;
    
    private RabbitHoleGroup gameplayObjects;
    private RabbitHoleDisplay viewport;
    
    // shorthand
    public RabbitHole RabbitHole => gameplayObjects.ObstacleContext;
    public Camera GameplayCam => gameplayObjects.GameplayCam;
    public RabbitHoleHUD UIOverlay => gameplayObjects.UIOverlay;

    public LevelConfig Config => levelConfig;
    
    public float fallSpeed
    {
        get
        {
            float baseline = MasterConfig.Values.BaseFallSpeed * levelConfig.FallSpeedMultiplier;

            // increase fall speed the farther down you are
            float depth = GetProgressTotal();
            return baseline + MasterConfig.Values.BaseFallAcceleration * levelConfig.FallAccelerationMultiplier * depth;
        }
    }

    #region serialized fields
    
    public SpriteRenderer[] BackgroundTiles => RabbitHole.BackgroundTiles;
    
    public float titleAnimationDistance => RabbitHole.titleAnimationDistance;
    public float titleAnimationDuration => MasterConfig.Values.titleAnimationDuration;
    public float introAnimationDistance => MasterConfig.Values.introAnimationDistance;
    public float outroAnimationDistance => MasterConfig.Values.outroAnimationDistance;
    public float introAnimationSpeed => MasterConfig.Values.introAnimationSpeed;
    public MenuGraphics menuGraphics => RabbitHole.menuGraphics;
    public AnimationCurve titleAnimationCurve => MasterConfig.Values.titleAnimationCurve;
    public AnimationCurve introAnimationCurve => MasterConfig.Values.introAnimationCurve;

    private Sprite[] perLevelBackgroundSprites => RabbitHole.perLevelBackgroundSprites;
    
    #endregion

    // fields
    public RabbitHoleDisplay Viewport => viewport;
    private float totalFallDistance;
    public float TotalFallDistance => totalFallDistance;
    private float initialHeight;
    public float InitialHeight => initialHeight;
    private float outroStartHeight;
    private readonly List<LevelChunk> activeChunks = new List<LevelChunk>();
    private readonly List<LevelCollider> activeObstacles = new List<LevelCollider>();
    private readonly List<LevelCollider> collisionBuffer = new List<LevelCollider>();
    private ChunkSpawner chunkSpawner;
    private float chunkCursor;
    private AnimationMode mode;
    private float multipurposeTimer;
    private float titleAnimationSpeed = 1f;
    private LevelConfig levelConfig;
    private Transform rabbitHoleObject => gameplayObjects.ObstacleContext.transform;
    // private Transform transform;

    public enum AnimationMode
    {
        Default,
        Interactive,
        // no player control
        Intro,
        Outro, 
        Title
    }

    private LevelChunk[] chunkPrefabs => levelConfig.ChunkSet.Chunks;

    // per instance values TODO rename
    public int VpLives { get; set; }
    public int VpScore { get; set; }

    public FallingGameInstance(RabbitHoleGroup gameplayObjects, RabbitHoleDisplay viewport)
    {
        this.gameplayObjects = gameplayObjects;
        this.viewport = viewport;
        
        // link
        gameplayObjects.ObstacleContext.AssociatedGameInstance = this;
        viewport.AssociatedGameInstance = this;

        // register
        if (!All.Contains(this)) All.Add(this);
        
        // initialize values
        if (levelConfig == default)
        {
            levelConfig = MasterConfig.Values.LevelConfigs[0]; // initial level
        }
        mode = AnimationMode.Title; // first load
        initialHeight = rabbitHoleObject.localPosition.y;
        Time.timeScale = 1f;
        multipurposeTimer = titleAnimationDuration;
        
        Reset();
    }

    public void Reset()
    {
        chunkSpawner = new ChunkSpawner(chunkPrefabs);

        VpLives = ApplicationLifetime.MAX_LIVES;

        chunkCursor = -LevelChunk.height;

        totalFallDistance = 0;

        // reset level height
        rabbitHoleObject.localPosition = new Vector3(rabbitHoleObject.localPosition.x, initialHeight, rabbitHoleObject.localPosition.z);

        // clean up game objects
        RemoveAllChunks();
    }

    public bool HandleGlobalEvent(GlobalGameEvent globalGameEvent)
    {
        switch (globalGameEvent)
        {
            case GlobalGameEvent.AllLivesLost:
                UIOverlay.GameOverOverlay.SetActive(true);
                break;
            case GlobalGameEvent.PlatformerLevelEndReached:
                PlayOutroAnimation();
                break;
        }

        return false;
    }

    public void OnShow()
    {
        World.Get<MainUIController>().SetGameViewVisible();
        World.Get<AliceCharacter>().ActivateGameplayMode();
        TimeDistortionController.SetBaselineSpeed(Current.Config.TimeScaleMultiplier);
        Reset();
        PlayIntroAnimationForCurrentLevel();
    }

    public void OnHide()
    {
        World.Get<MainUIController>().SetGameViewHidden();
        Reset();
        UIOverlay.GameOverOverlay.SetActive(false);
        menuGraphics.ShowStageArt(GameplayManager.SelectedLevel);
        PlayTitleIntro();
    }

    public void OnDisplayDestroyed()
    {
        All.Remove(this);
    }
    
    public float GetProgressTotal()
    {
        return rabbitHoleObject.localPosition.y - InitialHeight;
    }

    // runs every tick
    public void Tick()
    {
        if (mode == AnimationMode.Title) UpdateTitleAnim();
        else if (mode == AnimationMode.Intro) UpdateIntroAnim();
        else if (mode == AnimationMode.Outro) UpdateOutroAnim();
        else if (mode == AnimationMode.Interactive) UpdateInteractive();

        if (!ApplicationLifetime.IsGameplayPaused)
        {
            var player = World.Get<AliceCharacter>();

            // update active obstacles
            for (int i = activeObstacles.Count-1; i >= 0; i--)
            {
                if (activeObstacles[i] == null)
                    activeObstacles.RemoveAt(i);
            }

            // check collisions
            bool hasFocus = player.gameContext == this;
            if (hasFocus)
            {
                collisionBuffer.Clear();
                foreach (var obstacle in activeObstacles)
                {
                    bool ignoresInvincibility = obstacle.HasTag(LevelCollider.Tag_MoneyOnHit);
                    bool invincible = player.IsFlashing();
                    if (ignoresInvincibility || !invincible)
                    {
                        if (player.CheckOverlap(obstacle))
                        {
                            collisionBuffer.Add(obstacle);
                        }
                    }
                }
                foreach (LevelCollider hit in collisionBuffer)
                {
                    player.HandleObstacleCollision(hit);
                }
            }

            // spawn new obstacles
            // PerFrameVariableWatches.SetDebugQuantity("temp", (initialHeight - transform.localPosition.y).ToString() + " < " + (chunkCursor - LevelChunk.height).ToString());
            if (initialHeight - rabbitHoleObject.localPosition.y < chunkCursor + 6
                // stop spawning chunks before the level end
                && totalFallDistance < levelConfig.FallLength - LevelChunk.height * 2)
            {
                var newChunkPrefab = chunkSpawner.Force();
                LevelChunk newChunk = Object.Instantiate(newChunkPrefab, rabbitHoleObject);
                chunkCursor -= Mathf.Sign(fallSpeed) * LevelChunk.height;
                newChunk.transform.localPosition = new Vector3(0, chunkCursor - initialHeight, 0);
                activeChunks.Add(newChunk);
                activeObstacles.AddRange(newChunk.Obstacles);

                const int maxChunks = 3;
                if (activeChunks.Count > maxChunks)
                {
                    var oldest = activeChunks[0];
                    activeChunks.RemoveAt(0);
                    Object.Destroy(oldest.gameObject);
                }
            }

            // check game over condition
            if (hasFocus && mode == AnimationMode.Interactive && totalFallDistance > levelConfig.FallLength)
            {
                GameplayManager.Fire(GlobalGameEvent.PlatformerLevelEndReached);
            }

            // debug
            UpdateDebug();
        }
    }

    private void UpdateTitleAnim()
    {
        multipurposeTimer -= Time.deltaTime * titleAnimationSpeed;
        Vector3 camRootPos = Vector3.zero;
        float t = (titleAnimationDuration - multipurposeTimer) / titleAnimationDuration;
        float newY = Mathf.Lerp(camRootPos.y + titleAnimationDistance, camRootPos.y, titleAnimationCurve.Evaluate(t));
        Transform cameraTransform = Viewport.GameplayCamera.transform;
        cameraTransform.localPosition = new Vector3(camRootPos.x, newY, camRootPos.z);
        if (multipurposeTimer <= 0)
        {
            OnTitleAnimComplete();
        }
    }

    private void OnTitleAnimComplete()
    {
        titleAnimationSpeed = 1f;
        Viewport.GameplayCamera.transform.localPosition = Vector3.zero;
        mode = AnimationMode.Default;
        GameplayManager.Fire(GlobalGameEvent.MenuAnimationFinished);
    }

    private void UpdateIntroAnim()
    {
        rabbitHoleObject.localPosition += new Vector3(0, Time.deltaTime * fallSpeed, 0);
        if (rabbitHoleObject.localPosition.y > initialHeight + introAnimationDistance)
        {
            OnIntroComplete();
        }
    }

    private void UpdateOutroAnim()
    {
        rabbitHoleObject.localPosition += new Vector3(0, Time.deltaTime * fallSpeed, 0);

        // alice lerp to resting pos
        var player = World.Get<AliceCharacter>();
        bool hasFocus = player.gameContext == this;
        if (hasFocus)
        {
            World.Get<AliceCharacter>().IsHijacked = true;
            float t = (rabbitHoleObject.localPosition.y - outroStartHeight) / outroAnimationDistance;
            Vector3 characterTargetRestingPos = new Vector3(rabbitHoleObject.position.x, -2, 0);
            Transform aliceTransform = player.transform;
            aliceTransform.position = Vector3.Lerp(aliceTransform.position, characterTargetRestingPos, t);
        }

        if (rabbitHoleObject.localPosition.y > outroStartHeight + outroAnimationDistance)
        {
            OnOutroComplete();
        }
    }

    private void UpdateInteractive()
    {
        if (!ApplicationLifetime.IsGameplayPaused)
        {
            rabbitHoleObject.localPosition += new Vector3(0, Time.deltaTime * fallSpeed, 0);
            totalFallDistance = rabbitHoleObject.localPosition.y - initialHeight;
        }
    }

    private void UpdateDebug()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // shrink
            World.Get<AliceCharacter>().GetComponent<ShrinkBehavior>().OnShrink();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // grow
            World.Get<AliceCharacter>().GetComponent<ShrinkBehavior>().OnGrow();
        }
    }

    public void PlayIntroAnimationForRestart()
    {
        menuGraphics.HideAllStageArt();
        PlayIntroAnim();
    }

    public void PlayIntroAnimationForCurrentLevel()
    {
        menuGraphics.ShowStageArt(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
        PlayIntroAnim();
    }

    private void PlayIntroAnim()
    {
        mode = AnimationMode.Intro;
        SetBackgroundSpritesForLevel((int) ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value);
    }

    private void OnIntroComplete()
    {
        Viewport.Overlay?.SetActive(true);
        mode = AnimationMode.Interactive;
    }
    
    public void PlayOutroAnimation()
    {
        outroStartHeight = rabbitHoleObject.localPosition.y;
        mode = AnimationMode.Outro;
        //OwnerLink?.Overlay?.SetActive(false);
        menuGraphics.transform.localPosition = new Vector3(0, -outroStartHeight - outroAnimationDistance, 0);
        menuGraphics.ShowStageArt(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value+1);
    }

    private void OnOutroComplete()
    {
        World.Get<AliceCharacter>().IsHijacked = false;

        Viewport.Overlay?.SetActive(false);
        mode = AnimationMode.Default;

        // move the menu graphics to the new player position so that the player cannot tell that the height is back to zero
        menuGraphics.transform.localPosition = new Vector3(0, -initialHeight, 0);

        GameplayManager.Fire(GlobalGameEvent.PlatformerLevelEndAnimationFinished);
    }

    private void RemoveAllChunks()
    {
        // clean up game objects
        foreach (var chunk in activeChunks)
        {
            if (chunk != null) Object.Destroy(chunk.gameObject);
        }

        // clear queue
        activeChunks.Clear();
        activeObstacles.Clear();
    }

    private float GetIntroOffset()
    {
        return rabbitHoleObject.localPosition.y - (initialHeight + introAnimationDistance);
    }

    private void SetBackgroundSpritesForLevel(int levelIndex)
    {
        if (perLevelBackgroundSprites.TryGet(levelIndex, out Sprite art)) {
            foreach (var backgroundTile in BackgroundTiles)
            {
                backgroundTile.sprite = art;
            }
        }
        else
        {
            Debug.Log("failed to find background for level " + levelIndex);
        }
    }

    public void FastForwardTitleIntro()
    {
        titleAnimationSpeed = 20f;
    }

    public void PlayTitleIntro()
    {
        mode = AnimationMode.Title;
    }
}