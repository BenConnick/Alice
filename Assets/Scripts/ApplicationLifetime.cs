using System;
using UnityEngine;
using System.Collections.Generic;

public delegate void PlatformEventDelegate(int platformID);
public delegate void NoteEventDelegate(int noteID);
public delegate void EventDelegate();

// Application Manager
// static class, top control of the app state
public static partial class ApplicationLifetime
{
    #region state

    public static StateMachine<AppMode> Modes = new StateMachine<AppMode>();
    
    private static SerializablePlayerData _playerData;

    //private static FallingGameLifetime _fallingGame;
    
    public static SerializablePlayerData GetPlayerData()
    {
        return _playerData;
    }

    public static IGameLifetime GetActiveGameLifetime()
    {
        //return _fallingGame;
        throw new NotImplementedException();
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
        Modes.ChangeState(LoadingMode.Instance);
        
        // load
        _playerData = new SerializablePlayerData();
        _playerData.TryLoadFromDisk();
#if UNITY_EDITOR
        DebugOnPostInitialize();
#endif
        GameEventHandler.OnGameEvent(NavigationEvent.BootLoadFinished);
    }

    private static void InitializeAppModes(StateMachine<AppMode> modes)
    {
        LoadingMode.Instance = new LoadingMode(modes);
        TitleMenuMode.Instance = new TitleMenuMode(modes);
        LevelSelectMode.Instance = new LevelSelectMode(modes);
        PauseMenuMode.Instance = new PauseMenuMode(modes);
        FallingGameActiveMode.Instance = new FallingGameActiveMode(modes);
        FallingGameSpectatorMode.Instance = new FallingGameSpectatorMode(modes);
        PreFallCutsceneMode.Instance = new PreFallCutsceneMode(modes);
        PostFallCutsceneMode.Instance = new PostFallCutsceneMode(modes);
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

    public static void ChangeMode(AppMode nextMode)
    {
        Modes.ChangeState(nextMode);
        Debug.Log($"Game state changed to '{CurrentMode.Name}'");
    }
    #endregion


}
