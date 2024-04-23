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
        
        // begin loading
        ChangeMode<LoadingMode>();
        
        // load
        _playerData = new SerializablePlayerData();
        _playerData.TryLoadFromDisk();
#if UNITY_EDITOR
        DebugOnPostInitialize();
#endif
        GameplayManager.Fire(GlobalGameEvent.BootLoadFinished);
    }

    private static void InitializeAppModes(StateMachine<AppMode> modes)
    {
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

    public static void ChangeMode<T>() where T : AppMode, new()
    {
        AppMode appMode = Modes.Get<T>();
        if (appMode == CurrentMode)
        {
            Debug.LogError($"Current mode and next mode ({appMode.GetType()}) {appMode} are the same");
        }
        Modes.ChangeState(appMode);
        Debug.Log($"Game state changed to '{CurrentMode.Name}'");
    }

    public static void HandleGameEvent(GlobalGameEvent gameEvent)
    {
        Debug.Log("On game event: " + gameEvent);
        switch (gameEvent)
        {
            case GlobalGameEvent.BootLoadFinished:
            {
                ChangeMode<TitleMenuMode>();
                break;
            }
            case GlobalGameEvent.MainMenuGoNext:
            {
                ChangeMode<PreFallSlideshowMode>();
                break;
            }
            case GlobalGameEvent.PreRunCutsceneFinished:
            {
                ChangeMode<FallingGameActiveMode>();
                break;
            }
            case GlobalGameEvent.PlatformerLevelEndReached:
            case GlobalGameEvent.AllLivesLost:
            case GlobalGameEvent.MenuAnimationFinished:
            {
                CurrentMode.HandleGameEvent(gameEvent);
                break;
            }
            case GlobalGameEvent.PlatformerLevelEndAnimationFinished:
            {
                // begin dialogue
                ChangeMode<PostFallWinSlideshowMode>();
                break;
            }
            case GlobalGameEvent.GameResultsClosed:
            {
                // set mode to main menu
                ChangeMode<PostFallLoseSlideshowMode>();
                break;
            }
            case GlobalGameEvent.PostRunCutsceneFinished:
            {
                ChangeMode<LevelSelectMode>();
                break;
            }
            case GlobalGameEvent.LevelSelectionConfirmed:
            {
                ChangeMode<PreFallSlideshowMode>();
                break;
            }
        }
    }
}
