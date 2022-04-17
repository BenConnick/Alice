using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public delegate void PlatformEventDelegate(int platformID);
public delegate void NoteEventDelegate(int noteID);
public delegate void EventDelegate();

// Game Manager
// static class, top control of the game state
public static class GM
{
    public enum GameMode
    {
        Default,
        LoadingScreen, // unused - placeholder
        MainMenu,
        PauseMenu, // unused - placeholder
        Gameplay, // the main gameplay mode
        Scoreboard,
        EnterName,
    }

    public enum NavigationEvent
    {
        Default,
        StartButton,
        StartLevel,
        CutsceneEnded,
        PlatformerGameOver,
        PlatformerLevelUp,
        SkipIntroDialogue,
        OpenNamePicker,
        CloseNamePicker,
        CloseScoreboard,
        CheatCodeEntered,
    }

    public enum DebugEvent
    {
        Default,
        ShowNameEntryScreen,

    }

    // so-called "cheat codes"
    public static readonly IReadOnlyDictionary<LevelType, string> CheatCodes = new Dictionary<LevelType, string>()
    {
        { LevelType.Caterpillar, "DRINK" },
        { LevelType.CheshireCat, "GROWN" },
        { LevelType.MadHatter, "GRINS" },
        { LevelType.TweedleDum, "PARTY" },
        { LevelType.QueenOfHearts, "TWINS" },
    };

    public const int MAX_LIVES = 3;
    public static int Lives
    {
        get => lives;
        set
        {
            // set
            lives = value;

            // on lives changed
            //LivesChangedEvent.Invoke();

            // game over
            if (lives <= 0)
            {
                OnGameEvent(NavigationEvent.PlatformerGameOver);
            }
        }
    }
    public static LevelType CurrentLevel { get; set; }

    #region gamestate
    // Game State
    private static int lives = MAX_LIVES;
    public static bool IsGameplayPaused { get; private set; } = true;
    public static bool InputFrozen => IsGameplayPaused;
    public static bool FellThroughFloor { get; set; }
    public static GameMode CurrentMode { get; private set; }
    public static int CurrentScore { get; set; }
    public static readonly List<int> PlayerHighScores = new List<int>();
    public static readonly List<string> PlayerHighScoreNames = new List<string>(); // assumed same length as scores^
    #endregion

    // Screen Quick References
    public static GameObject MainMenu => helperObject.MainMenu;
    public static GameObject Scoreboard => helperObject.ScoreBoard;
    public static GameObject EnterNameScreen => helperObject.EnterNameScreen;

    // Helper object
    private static GMHelperObject helperObject;

    public interface IGameplayUI
    {

    }

    public static void Init(GMHelperObject helper)
    {
        if (helperObject != null || helper == null) return;

        Physics.autoSimulation = false;
        Physics2D.autoSimulation = false;

        helperObject = helper;

        ChangeMode(GameMode.MainMenu);
    }

    public static void InitEditor()
    {
        // load data
    }

    public static void Tick()
    {
        Tween.UpdateAll();
    }

    public static void LateTick()
    {

    }

    public static void OnRestart()
    {
        Lives = MAX_LIVES;
        //CameraController.SetY(0);
        // TODO reset player
    }

    public static void OnRetry()
    {
        Lives = MAX_LIVES;
        // TODO reset player
        IsGameplayPaused = false;
        ChangeMode(GameMode.Gameplay);
    }

    private static void SetLevel(LevelType levelType)
    {
       lives = 3;
       // PlatformManager.PlayLevel(Level);
       // CameraController.SetY(-PlatformManager.PrebakeDistance);
       // PC.transform.position = new Vector3(0, 0, 0);
    }

    private static void ChangeMode(GameMode mode)
    {
        GameObject activeScreen = null;
        switch (mode)
        {
            case GameMode.MainMenu:
                activeScreen = MainMenu;
                break;
            case GameMode.Gameplay:
                IsGameplayPaused = false;
                break;
            case GameMode.Scoreboard:
                IsGameplayPaused = true;
                activeScreen = Scoreboard;
                break;
            case GameMode.EnterName:
                IsGameplayPaused = true;
                activeScreen = EnterNameScreen;
                break;
            default:
                throw new Exception("unhandled mode");
        }

        // activate screens
        void ShowHide(GameObject g)
        {
            g.SetActive(g == activeScreen);
        }
        ShowHide(MainMenu);
        ShowHide(Scoreboard);
        ShowHide(EnterNameScreen);

        // update mode
        CurrentMode = mode;
    }

    public static void OnCheatCode(LevelType unlockCode)
    {
        switch (CurrentLevel)
        {
            case LevelType.RabbitHole:
                if (unlockCode == LevelType.Caterpillar)
                {
                    CurrentLevel = unlockCode;
                }
                break;
            case LevelType.Caterpillar:
                if (unlockCode == LevelType.CheshireCat)
                {
                    CurrentLevel = unlockCode;
                }
                break;
            case LevelType.CheshireCat:
                if (unlockCode == LevelType.MadHatter)
                {
                    CurrentLevel = unlockCode;
                }
                break;
            case LevelType.MadHatter:
                if (unlockCode == LevelType.TweedleDum)
                {
                    CurrentLevel = unlockCode;
                }
                break;
            case LevelType.TweedleDum:
                if (unlockCode == LevelType.QueenOfHearts)
                {
                    CurrentLevel = unlockCode;
                }
                break;
            case LevelType.QueenOfHearts:
                // TBD
                break;
            default:
                break;
        }
    }

    public static void OnGameEvent(NavigationEvent e)
    {
        switch (CurrentMode)
        {
            case GameMode.MainMenu:
                if (e == NavigationEvent.StartButton)
                {
                    if (CurrentLevel == LevelType.Default) CurrentLevel = LevelType.RabbitHole;
                    SetLevel(CurrentLevel);
                    FindSingle<RabbitHole>().Reset();
                    ChangeMode(GameMode.Gameplay);
                }
                break;
            case GameMode.Gameplay:
                if (e == NavigationEvent.PlatformerGameOver)
                {
                    ChangeMode(GameMode.Scoreboard);
                }
                break;
            case GameMode.Scoreboard:
                if (e == NavigationEvent.OpenNamePicker)
                {
                    ChangeMode(GameMode.EnterName);
                }
                else if (e == NavigationEvent.CloseScoreboard)
                {
                    ChangeMode(GameMode.MainMenu);
                }
                break;
            case GameMode.EnterName:
                if (e == NavigationEvent.CloseNamePicker)
                {
                    ChangeMode(GameMode.Scoreboard);
                }
                break;
            default:
                break;
        }
    }

    public static void OnDebugEvent(DebugEvent debugEvent)
    {
        switch (debugEvent)
        {
            case DebugEvent.ShowNameEntryScreen:
                CurrentLevel = LevelType.RabbitHole;
                CurrentScore = 110;
                ChangeMode(GameMode.EnterName);
                break;
            default:
                break;
        }
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return helperObject.StartCoroutine(routine);
    }

    // cached singletons for ease of reference
    private static readonly Dictionary<System.Type, MonoBehaviour> gameplayComponentsCache = new Dictionary<System.Type, MonoBehaviour>();
    // finds a single object in the scene with the requested component and caches it
    // for easily obtaining a reference to a singleton behavior across scripts without serializing
    public static T FindSingle<T>() where T : MonoBehaviour
    {
        // cached
        if (gameplayComponentsCache.ContainsKey(typeof(T)))
            return (T)gameplayComponentsCache[typeof(T)];
        // slow search
        var found = UnityEngine.Object.FindObjectOfType<T>();
        gameplayComponentsCache[typeof(T)] = found;
        return found;
    }
}
