using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public delegate void PlatformEventDelegate(int platformID);
public delegate void NoteEventDelegate(int noteID);
public delegate void EventDelegate();

// Game Manager
// static class, top control of the game state
public static partial class GM
{
    public enum GameMode
    {
        Default,
        LoadingScreen, // placeholder
        MainMenu,
        PauseMenu, // placeholder
        Gameplay, // the main gameplay mode
        Dialogue, // gameplay sub-mode
        GameOver,
        PreMainMenu, // animating in
    }

    public const string PassageStartPrefix = "Down the Rabbit Hole";
    public const string Caterpillar = "Conversation With A Caterpillar";
    public static int MAX_LIVES = 3;
    //public static MenuStage CurrentMenuStage { get; set; }
    public static LevelType CurrentLevel { get; set; }

    #region gamestate
    // Game State
    public static bool IsGameplayPaused => CurrentMode != GameMode.Gameplay;
    public static bool InputFrozen => IsGameplayPaused;
    public static GameMode CurrentMode { get; private set; }
    public static int CurrentScore { get; set; }
    public static readonly List<int> PlayerHighScores = new List<int>();
    public static readonly List<string> PlayerHighScoreNames = new List<string>(); // assumed same length as scores^
    public static int Money { get; set; }
    public static int DeathCount = 0;
    #endregion

    // Shorthand for screen objects
    public static GameObject MainMenu => helperObject.MainMenu;
    public static GameObject GameplayScreen => helperObject.Gameplay;
    public static GameObject Scoreboard => helperObject.ScoreBoard;
    public static GameObject EnterNameScreen => helperObject.EnterNameScreen;

    // Helper object
    private static GMHelperObject helperObject;

    public static void Init(GMHelperObject helper)
    {
        helperObject = helper;
        CurrentMode = GameMode.PreMainMenu;
        Application.targetFrameRate = 60;

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

    public static float GetLevelLength(LevelType level)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("ShortLevels"))
            return 5f;
#endif
        int l = (int)level;

        if (l >= helperObject.LevelLengths.Length)
        {
            return 100f;
        }

        return helperObject.LevelLengths[l];
    }


    private static float GetLevelTimeScale(LevelType level)
    {
        return level == LevelType.Caterpillar ? .5f : 1f;
    }
}
