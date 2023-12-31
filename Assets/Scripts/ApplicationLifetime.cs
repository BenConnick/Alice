using System;
using UnityEngine;
using System.Collections.Generic;

public delegate void PlatformEventDelegate(int platformID);
public delegate void NoteEventDelegate(int noteID);
public delegate void EventDelegate();

// Game Manager
// static class, top control of the game state
public static partial class ApplicationLifetime
{
    #region state
    public enum GameMode
    {
        Default,
        LoadingScreen, // placeholder
        MainMenu,
        LevelSelect,
        PauseMenu, // placeholder
        Gameplay, // the main gameplay mode
        Dialogue, // gameplay sub-mode
        GameOver,
        PreMainMenu, // animating in
    }
    
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
    public static bool IsGameplayPaused => CurrentMode != GameMode.Gameplay;
    public static GameMode CurrentMode { get; set; }
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
        helperObject = helper;
        CurrentMode = GameMode.PreMainMenu;
        Application.targetFrameRate = 60;

        _playerData = new SerializablePlayerData();
        _playerData.TryLoadFromDisk();

#if UNITY_EDITOR
        MAX_LIVES = UnityEditor.EditorPrefs.GetBool("OneLife") ? 1 : MAX_LIVES;
#endif
    }

    public static void InitEditor()
    {
        // load data
    }

    public static void Tick()
    {
        Tween.UpdateAll();
        ContextualInputSystem.Update();
    }

    public static void LateTick()
    {

    }
    #endregion


}
