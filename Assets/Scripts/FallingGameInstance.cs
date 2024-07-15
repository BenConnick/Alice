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
    private float titleAnimationTimer;
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
        Title,
        GameOver,
    }

    private LevelChunk[] chunkPrefabs => levelConfig.ChunkSet.Chunks;

    // per instance values TODO rename
    public int VpLives
    {
        get => vpLives;
        set
        {
            vpLives = value;
            VpScore.Hearts = value;
        }
    }

    public ScoreObject VpScore = new ScoreObject();
    private int vpLives;

    public bool IsPaused { get; set; }

    public FallingGameInstance(RabbitHoleGroup gameplayObjects, RabbitHoleDisplay viewport)
    {
        this.gameplayObjects = gameplayObjects;
        this.viewport = viewport;
        
        // link
        gameplayObjects.ObstacleContext.AssociatedGameInstance = this;
        viewport.AssociatedGameInstance = this;

        // register
        if (!All.Contains(this)) All.Add(this);
        
        // initialize starting values
        mode = AnimationMode.Title; // first load
        initialHeight = rabbitHoleObject.localPosition.y;
        Time.timeScale = 1f;
        titleAnimationTimer = titleAnimationDuration;
        
        Reset();
    }

    public void Reset()
    {
        if (mode != AnimationMode.Title)
        {
            mode = AnimationMode.Intro;
        }
        
        levelConfig = GameplayManager.GetCurrentLevelConfig();
        
        chunkSpawner = new ChunkSpawner(chunkPrefabs);

        VpLives = ApplicationLifetime.MAX_LIVES;

        chunkCursor = -LevelChunk.height;

        VpScore.Clear();

        totalFallDistance = 0;

        // reset level height
        rabbitHoleObject.localPosition = new Vector3(rabbitHoleObject.localPosition.x, initialHeight, rabbitHoleObject.localPosition.z);

        // reset title time?
        titleAnimationTimer = titleAnimationDuration;

        // clean up game objects
        RemoveAllChunks();
        
        World.Get<AliceCharacter>().StopFlashing();
    }

    public bool HandleGlobalEvent(GlobalGameEvent globalGameEvent)
    {
        switch (globalGameEvent)
        {
            case GlobalGameEvent.AllLivesLost:
                UIOverlay.GameOverOverlay.SetActive(true);
                mode = AnimationMode.GameOver;
                IsPaused = true;
                break;
            case GlobalGameEvent.PlatformerLevelEndReached:
                PlayOutroAnimation();
                break;
        }

        return false;
    }

    public void ReloadLevelParts()
    {
        levelConfig = GameplayManager.GetCurrentLevelConfig();
        chunkSpawner = new ChunkSpawner(chunkPrefabs);
    }

    public void OnShow()
    {
        World.Get<MainUIController>().SetGameViewVisible();
        World.Get<AliceCharacter>().ActivateGameplayMode();
        TimeDistortionController.SetBaselineSpeed(Config.TimeScaleMultiplier);
        Reset();
        IsPaused = false;
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
        if (IsPaused)
        {
            PausedTick();
            return;
        }
        
        // position update logic - animations focus-independent
        UpdateLevelPosition();
        bool hasFocus = World.Get<AliceCharacter>().gameContext == this;
        if (hasFocus) ForegroundUpdate();
        else BackgroundUpdate();
    }

    private void PausedTick()
    {
        
    }

    private void UpdateLevelPosition()
    {
        if (mode == AnimationMode.Title) UpdateTitleAnim();
        else if (mode == AnimationMode.Intro) UpdateIntroAnim();
        else if (mode == AnimationMode.Outro) UpdateOutroAnim();
        else if (mode == AnimationMode.Interactive)
        {
            Vector3 rabbitHolePos = rabbitHoleObject.localPosition;
            Vector3 newPos = rabbitHolePos + new Vector3(0, Time.deltaTime * fallSpeed, 0);
            rabbitHoleObject.localPosition = newPos;
            totalFallDistance = newPos.y - initialHeight;
        }
    }

    private void ForegroundUpdate()
    {
        // update active obstacles
        for (int i = activeObstacles.Count-1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
                activeObstacles.RemoveAt(i);
        }
        
        // check collisions
        var player = World.Get<AliceCharacter>();
        bool invincible = player.IsFlashing() || player.IsHijacked || totalFallDistance < LevelChunk.height*3;
        collisionBuffer.Clear();
        foreach (var obstacle in activeObstacles)
        {
            bool ignoresInvincibility = obstacle.HasTag(LevelCollider.Tag_MoneyOnHit);
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
            HandlePlayerObstacleCollision(player, hit);
        }


        // spawn new obstacles
        // PerFrameVariableWatches.SetDebugQuantity("temp", (initialHeight - transform.localPosition.y).ToString() + " < " + (chunkCursor - LevelChunk.height).ToString());
        if (initialHeight - rabbitHoleObject.localPosition.y < chunkCursor + 6)
        {
            var newChunkPrefab = chunkSpawner.Force();
            LevelChunk newChunk = Object.Instantiate(newChunkPrefab, rabbitHoleObject);
            chunkCursor -= Mathf.Sign(fallSpeed) * LevelChunk.height;
            newChunk.transform.localPosition = new Vector3(0, chunkCursor - initialHeight, 0);
            activeChunks.Add(newChunk);
            activeObstacles.AddRange(newChunk.Obstacles);

            const int maxChunks = 4;
            if (activeChunks.Count > maxChunks)
            {
                var oldest = activeChunks[0];
                activeChunks.RemoveAt(0);
                Object.Destroy(oldest.gameObject);
            }
        }

        // (obsolete: endless) check game over condition
        // if (false)
        // {
        //     GameplayManager.Fire(GlobalGameEvent.PlatformerLevelEndReached);
        // }

        if (totalFallDistance > 20 && ApplicationLifetime.GetPlayerData().LastSelectedLevel.Value == LevelType.RabbitHole)
        {
            player.GiveTemporaryInvincibility(2.5f);
            GameplayManager.Fire(GlobalGameEvent.Temp);
            Vector3 rabbitHolePos = rabbitHoleObject.localPosition;
            Vector3 newPos = rabbitHolePos + new Vector3(0, LevelChunk.height*2, 0);
            rabbitHoleObject.localPosition = newPos;
            totalFallDistance = newPos.y - initialHeight;
        }
        
        // score
        VpScore.Meters = Mathf.FloorToInt(GetProgressTotal()); // <- putting the actual score in the UI rendering is questionable at best...
        
        // debug
        UpdateDebug();
    }

    private void HandlePlayerObstacleCollision(AliceCharacter player, LevelCollider obstacle) 
    {
        if (obstacle.HasTag(LevelCollider.Tag_DamageOnHit))
        {
            // invert the collider
            var inv = obstacle.gameObject.AddComponent<InvertSpriteTintBehavior>();
            inv.Initialize();

            // flash the collider
            //var flashing = obstacle.gameObject.AddComponent<FlashingBehavior>();
            //flashing.flashOffTime = 0.07f;
            //flashing.StartFlashing();

            // bump up the removal time (if applicable)
            var destroyer = obstacle.gameObject.GetComponent<DestroyAfterTimeBehavior>();
            if (destroyer != null) destroyer.SecondsUntilDestruction = Mathf.Min(destroyer.SecondsUntilDestruction, 2);

            // shake, flash, subtract lives
            Viewport.InvertColor(0.01f);
            TimeDistortionController.PlayImpactFrame(.9f);
            Viewport.GameplayCamera.GetComponent<GameplayInnerDisplayCamera>().Shake(); // DISABLED FOR EDITING
            player.StartFlashing();
            SubtractLife();
        }
        if (obstacle.HasTag(LevelCollider.Tag_GrowOnHit))
        {
            player.GetComponent<ShrinkBehavior>().OnGrow();
        }
        if (obstacle.HasTag(LevelCollider.Tag_ShrinkOnHit))
        {
            player.GetComponent<ShrinkBehavior>().OnShrink();
        }
        if (obstacle.HasTag(LevelCollider.Tag_MoneyOnHit))
        {
            GameplayManager.AddMoney(1);
            obstacle.gameObject.SetActive(false);
        }
    }

    private void SubtractLife()
    {
        // per the current design, lives and score are NOT global,
        // this is different from a normal game
        // the game has multiple contexts, with different lives and score
        // late in the game, the player can switch between these contexts
        // and in doing so can effectively gain or lose lives
        // this may eventually switch to a global context if that idea doesn't end up in the game
        VpLives--;
        if (VpLives <= 0)
        {
            GameplayManager.Fire(GlobalGameEvent.AllLivesLost);
        }
    }

    private void BackgroundUpdate()
    {
        // unused
    }

    private void UpdateTitleAnim()
    {
        titleAnimationTimer -= Time.deltaTime * titleAnimationSpeed;
        Vector3 camRootPos = Vector3.zero;
        float t = (titleAnimationDuration - titleAnimationTimer) / titleAnimationDuration;
        float newY = Mathf.Lerp(camRootPos.y + titleAnimationDistance, camRootPos.y, titleAnimationCurve.Evaluate(t));
        Transform cameraTransform = Viewport.GameplayCamera.transform;
        cameraTransform.localPosition = new Vector3(camRootPos.x, newY, camRootPos.z);
        if (titleAnimationTimer <= 0)
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
        float distanceFromAnimationStart = rabbitHoleObject.localPosition.y - outroStartHeight;
        float t = distanceFromAnimationStart / outroAnimationDistance;
        rabbitHoleObject.localPosition += new Vector3(0, Time.deltaTime * fallSpeed * (1-t), 0);

        // alice lerp to resting pos
        var player = World.Get<AliceCharacter>();
        bool hasFocus = player.gameContext == this;
        if (hasFocus)
        {
            player.IsHijacked = true;
            Vector3 alicePosition = player.transform.position;
            player.transform.position = alicePosition + Vector3.down * (Time.deltaTime * 2 * fallSpeed * t);
            if (alicePosition.y < -11)
            {
                OnOutroComplete();
            }
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

    public void HandlePlayerInput(ContextualInputSystem.InputType inputType)
    {
        if (inputType == ContextualInputSystem.InputType.MouseUp)
        {
            if (mode == AnimationMode.GameOver)
            {
                GameplayManager.Fire(GlobalGameEvent.GameResultsClosed);
            }
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
        SetBackgroundSpritesForLevel();
    }

    private void OnIntroComplete()
    {
        Viewport.Overlay?.gameObject?.SetActive(true);
        mode = AnimationMode.Interactive;
    }
    
    public void PlayOutroAnimation()
    {
        outroStartHeight = rabbitHoleObject.localPosition.y;
        mode = AnimationMode.Outro;
        
        // OBSOLETE
        //OwnerLink?.Overlay?.SetActive(false);
        //menuGraphics.transform.localPosition = new Vector3(0, -outroStartHeight - outroAnimationDistance, 0);
        //menuGraphics.ShowStageArt(ApplicationLifetime.GetPlayerData().LastUnlockedLevel.Value+1);
    }

    private void OnOutroComplete()
    {
        World.Get<AliceCharacter>().IsHijacked = false;

        Viewport.Overlay?.gameObject?.SetActive(false);
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

    private void SetBackgroundSpritesForLevel()
    {
        var art = GameplayManager.GetCurrentLevelConfig()?.BackgroundSprite;
        if (art == null)
        {
            Debug.Log("failed to find background for level " + GameplayManager.GetCurrentLevelConfig()?.LevelType);
            art = RabbitHole.fallbackBackgroundSprite;
        }
        foreach (var backgroundTile in BackgroundTiles)
        {
            backgroundTile.sprite = art;
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