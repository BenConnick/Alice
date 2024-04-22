using System;
using UnityEngine;
using System.Collections.Generic;

// Application Manager
// static class, top control of the app state
public static partial class ApplicationLifetime
{
    #region state

    public static StateMachine<AppMode> Modes = new StateMachine<AppMode>();
    
    private static SerializablePlayerData _playerData;

    #endregion
    
    public static SerializablePlayerData GetPlayerData()
    {
        return _playerData;
    }

    public static int MAX_LIVES = 3;

    #region gamestate
    // Game State
    public static bool IsGameplayPaused => !(CurrentMode is FallingGameActiveMode);
    public static AppMode CurrentMode => (AppMode)Modes.CurrentState;
    public static int CurrentScore { get; set; }
    public static readonly List<int> PlayerHighScores = new List<int>();
    public static readonly List<string> PlayerHighScoreNames = new List<string>(); // assumed same length as scores^
    #endregion

    // Helper object
    private static BootstrapObject helperObject;

    public static BootstrapObject GetGameObject()
    {
        return helperObject;
    }

    public static void Init(BootstrapObject helper)
    {
        // link bootstrap
        helperObject = helper;
        
        // init frame rate
        Application.targetFrameRate = 60;
        
        // init modes
        InitializeAppModes(Modes);
        
        // begin loading
        ChangeMode<LoadingMode>();
        
        // load
        _playerData = new SerializablePlayerData();
        _playerData.TryLoadFromDisk();
#if UNITY_EDITOR
        DebugOnPostInitialize();
#endif
        Nav.Go(NavigationEvent.BootLoadFinished);
    }

    private static void InitializeAppModes(StateMachine<AppMode> modes)
    {
        new LoadingMode(modes);
        new TitleMenuMode(modes);
        new LevelSelectMode(modes);
        new PauseMenuMode(modes);
        new FallingGameActiveMode(modes);
        new FallingGameSpectatorMode(modes);
        new GameResultsMenuMode(modes);
        new PreFallCutsceneMode(modes);
        new PostFallWinCutsceneMode(modes);
        new PostFallLoseCutsceneMode(modes);
        
        // additional initialization
        Modes.Get<PreFallCutsceneMode>().DialogueExhausted += () => Nav.Go(NavigationEvent.PreRunDialogueFinished);
        Modes.Get<PostFallWinCutsceneMode>().DialogueExhausted += () => Nav.Go(NavigationEvent.PostRunDialogueFinished);
        Modes.Get<PostFallLoseCutsceneMode>().DialogueExhausted += () => Nav.Go(NavigationEvent.PostRunDialogueFinished);
    }

    public static void InitEditor()
    {
        // load data
    }

    public static void Tick()
    {
        ContextualInputSystem.Update();
        Modes.Tick(Time.deltaTime);
        Tween.UpdateAll();
    }

    public static void LateTick()
    {

    }

    public static void ChangeMode<T>() where T : AppMode
    {
        if (!Modes.RegisteredStates.TryGetValue(typeof(T), out AppMode appMode)) 
        {
            Debug.LogError($"Mode {typeof(T)} not registered");
            return;
        }
        if (appMode == CurrentMode)
        {
            Debug.LogError($"Current mode and next mode ({appMode.GetType()}) {appMode} are the same");
        }
        Modes.ChangeState(appMode);
        Debug.Log($"Game state changed to '{CurrentMode.Name}'");
    }

    public static void ChangeSelectedLevel(LevelType newSelection)
    {
        if (newSelection <= _playerData.LastUnlockedLevel.Value)
        {
            _playerData.LastSelectedLevel.Set(newSelection);
        }
    }
}
