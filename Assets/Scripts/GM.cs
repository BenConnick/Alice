using System.Collections;
using UnityEngine;
using System;
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
        // LoadingScreen, // unused - placeholder
        MainMenu,
        // PauseMenu, // unused - placeholder
        Gameplay, // the main gameplay mode
        Dialogue, // gameplay sub-mode
        //Scoreboard, // deprecated
        //EnterName, // deprecated
        //GameOver, // deprecated
    }

    // Like LevelType, but more detailed
    // the main-menu changes as you play
    // it has the following appearance options
    //public enum MenuStage
    //{
    //    PreRabbitHole,
    //    RabbitHole,
    //    DrinkMe,
    //    Caterpillar,
    //    Chess,
    //    TeaParty,
    //    QueenStart,
    //    QueenDungeon,
    //    GameWinMenu,
    //}

    public enum NavigationEvent
    {
        Default,
        GoButton,
        PlatformerGameOver,
        PlatformerLevelEndTrigger,
        PlatformerLevelEndPostAnimation,
        CheatCodeEntered,
        FallFromMonologue,
    }

    public enum DebugEvent
    {
        Default,
        ShowNameEntryScreen,
        SetLevelCaterpillar,
    }

    public const string PassageStartPrefix = "Down the Rabbit Hole";
    public const string Caterpillar = "Conversation With A Caterpillar";
    public static int MAX_LIVES = 3;
    //public static MenuStage CurrentMenuStage { get; set; }
    public static LevelType CurrentLevel { get; set; }

    #region gamestate
    // Game State
    public static bool IsGameplayPaused { get; private set; } = true;
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
        Application.targetFrameRate = 60;
        CurrentMode = GameMode.MainMenu;
        ChangeActiveScreen(GameMode.MainMenu);
#if UNITY_EDITOR
        MAX_LIVES = UnityEditor.EditorPrefs.GetBool("OneLife") ? 1 : 3;
#endif
        FindSingle<SplitGameplayMomentAnimationController>().SetToDefaultState();
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

    private static void ChangeActiveScreen(GameMode mode)
    {
        GameObject activeScreen = null;
        switch (mode)
        {
            case GameMode.MainMenu:
                activeScreen = GameplayScreen;
                break;
            case GameMode.Gameplay:
                activeScreen = GameplayScreen;
                break;
            // deprecated
            //case GameMode.Scoreboard:
            //    IsGameplayPaused = true;
            //    activeScreen = Scoreboard;
            //    break;
            //case GameMode.EnterName:
            //    IsGameplayPaused = true;
            //    activeScreen = EnterNameScreen;
            //    break;
            default:
                throw new Exception("unhandled mode: " + mode);
        }

        // activate screens
        void ShowHide(GameObject g)
        {
            g.SetActive(g == activeScreen);
        }
        ShowHide(MainMenu);
        ShowHide(GameplayScreen);
        // ShowHide(Scoreboard); deprecated
        // ShowHide(EnterNameScreen); deprecated
    }

    public static float GetLevelLength(LevelType level)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("ShortLevels"))
            return 5f;
#endif
        switch (level)
        {
            case LevelType.Default:
                break;
            case LevelType.RabbitHole:
                return 300f;
            case LevelType.Caterpillar:
                return 30f;
            case LevelType.CheshireCat:
                break;
            case LevelType.MadHatter:
                break;
            case LevelType.QueenOfHearts:
                break;
            default:
                break;
        }
        return 10f;
    }

    public static void OnGameEvent(NavigationEvent gameEvent)
    {
        switch (gameEvent)
        {
            case NavigationEvent.GoButton:
                HandleGoButtonInGlobalContext();
                break;
            case NavigationEvent.PlatformerGameOver:
                DoGameOver();
                break;
            case NavigationEvent.PlatformerLevelEndTrigger:
                ShowLevelEndAnimation();
                break;
            case NavigationEvent.PlatformerLevelEndPostAnimation:
                DoEnterDialogueArea();
                break;
            case NavigationEvent.CheatCodeEntered:
                // deprecated
                break;
            case NavigationEvent.FallFromMonologue:
                var rh = FindSingle<RabbitHole>();
                rh.Reset();
                rh.PlayIntroAnimationForRestart();
                IsGameplayPaused = false;
                FindSingle<GameplayScreenBehavior>().ShowGame();
                break;
            default:
                throw new Exception("Unhandled game event: " + gameEvent);
        }
    }

    #region game event handlers
    private static void HandleGoButtonInGlobalContext()
    {
        switch (CurrentMode)
        {
            case GameMode.Dialogue:
                AdvanceDialogueContext();
                break;
            case GameMode.MainMenu:
                DoFirstStart();
                FindSingle<Alice>().UnbecomeButton();
                break;
            case GameMode.Gameplay:
                break;
        }
    }

    private static void AdvanceDialogueContext()
    {
        // get current dialogue context
        var ctx = FindSingle<CharacterDialogueBehavior>(); // TODO
        // advance to next step
        bool last = ctx.PlayNextLine();
        // if this is the last step, start the next falling section
        if (last)
        {
            FindSingle<Alice>().UnbecomeButton();
            IsGameplayPaused = false;
            FindSingle<RabbitHole>().PlayIntroAnimationForCurrentLevel();
        }
    }

    private static void DoFirstStart()
    {
        CurrentMode = GameMode.Gameplay;
        CurrentLevel = LevelType.RabbitHole;
        foreach (var r in UnityEngine.Object.FindObjectsOfType<RabbitHole>())
        {
            r.Reset();
        }
        ChangeActiveScreen(GameMode.Gameplay);
        IsGameplayPaused = false;
        FindSingle<RabbitHole>().PlayIntroAnimationForCurrentLevel();
    }

    private static void DoGameOver()
    {
        DeathCount++;
        IsGameplayPaused = true;
        string storyKey = PassageStartPrefix + (DeathCount < 2 ? "" : " " + DeathCount);
        ShowStory(storyKey);
    }

    private static void ShowLevelEndAnimation()
    {
        FindSingle<RabbitHole>().PlayOutroAnimation();
    }

    private static void DoEnterDialogueArea()
    {
        CurrentMode = GameMode.Dialogue;
        CurrentLevel++;
        IsGameplayPaused = true;
        FindSingle<Alice>().BecomeButton();
        AdvanceDialogueContext();
    }
    #endregion

    private static void ShowStory(string storyKey)
    {
        FindSingle<GameplayScreenBehavior>().ShowStory(storyKey);
        CurrentMode = GameMode.Dialogue;
    }

    public static void OnDebugEvent(DebugEvent debugEvent)
    {
        Debug.Log("Debug Event: " + debugEvent);
        switch (debugEvent)
        {
            case DebugEvent.ShowNameEntryScreen:
                Debug.Log("Deprecated");
                //CurrentLevel = LevelType.RabbitHole;
                //CurrentScore = 110;
                //ChangeMode(GameMode.EnterName);
                break;
            case DebugEvent.SetLevelCaterpillar:
                CurrentLevel = LevelType.Caterpillar;
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
        T found = null;
        for (int i = 0; found == null && i < helperObject.SearchRoots.Length; i++)
        {
            var all = helperObject.SearchRoots[i].GetComponentsInChildren<T>(true);
            if (all.Length > 0) found = all[0];
        }

        // cache
        if (found != null)
            gameplayComponentsCache[typeof(T)] = found;

        return found;
    }

    // reminder that you can add debug commands
    // directly to this class and they will be added
    [Command]
    public static void Toot()
    {
        Debug.Log("toot");
    }
}
