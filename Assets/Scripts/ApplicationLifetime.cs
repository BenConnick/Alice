using System;
using UnityEngine;
using System.Collections.Generic;

// Application Manager
// static class, top control of the app state
public static partial class ApplicationLifetime
{
    #region state

    public static StateMachine<AppMode> Modes = new StateMachine<AppMode>();
    
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void EditorReload()
    {
        Modes = new StateMachine<AppMode>();
    }
    #endif
    
    private static SerializablePlayerData _playerData;

    #endregion
    
    public static SerializablePlayerData GetPlayerData()
    {
        return _playerData;
    }

    public static int MAX_LIVES = 1;

    #region gamestate
    // Game State
    public static bool IsGameplayPaused => (!(CurrentMode is FallingGameActiveMode) || FallingGameInstance.Current.IsPaused);
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

        //_playerData.LastSelectedLevel.Set(_playerData.LastUnlockedLevel.Value);

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
            Debug.LogWarning($"Current mode and next mode are the same: ({appMode.GetType()}) {appMode}");
        }
        EditorPersistentDebugOutput.UpdateDebugMessage("AppMode",$"current: '{appMode?.Name}', previous: '{CurrentMode?.Name}'");
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
                ChangeMode<FallingGameActiveMode>();
                break;
            }
            case GlobalGameEvent.PlatformerLevelEndReached:
            case GlobalGameEvent.AllLivesLost:
            case GlobalGameEvent.MenuAnimationFinished:
            {
                // handled in FallingGameInstance.HandleGlobalEvent()
                CurrentMode.HandleGameEvent(gameEvent);
                break;
            }
            case GlobalGameEvent.PlatformerLevelEndAnimationFinished:
            {
                // begin dialogue
                Debug.LogWarning("Obsolete event");
                break;
            }
            case GlobalGameEvent.GameResultsClosed:
            {
                // set mode to main menu
                ChangeMode<TitleMenuMode>();
                break;
            }
        }
    }
}
