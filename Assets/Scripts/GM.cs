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
        GameOver // TODO replace isGameOver
    }

    public enum GameEvent
    {
        Default,
        StartButton,
        StartLevel,
        CutsceneEnded,
        PlatformerGameOver,
        PlatformerLevelUp,
        SkipIntroDialogue,
    }

    public const int MAX_LIVES = 3;
    private static int lives = MAX_LIVES;
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
                OnGameOverCondition();
            }
        }
    }
    public static LevelType LevelType => (LevelType)Level;
    public static int Level
    {
        get;
        private set;
    }
    public static bool IsGameOver { get; private set; }
    public static bool IsGameplayPaused { get; private set; } = true;
    public static bool InputFrozen => IsGameplayPaused || IsGameOver;
    public static bool FellThroughFloor { get; set; }
    public static GameMode CurrentMode { get; private set; }

    public static GameObject MainMenu => helperObject.MainMenu;

    private static GMHelperObject helperObject;

    public interface IGameplayUI
    {

    }

    public static void Init(GMHelperObject helper)
    {
        if (helperObject != null || helper == null) return;

        helperObject = helper;

        IsGameOver = false;

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
        IsGameOver = false;
        Lives = MAX_LIVES;
        //CameraController.SetY(0);
        // TODO reset player
    }

    public static void OnRetry()
    {
        IsGameOver = false;
        Lives = MAX_LIVES;
        // TODO reset player
        IsGameplayPaused = false;
        ChangeMode(GameMode.Gameplay);
    }

    private static void OnGameOverCondition()
    {
        if (IsGameOver) return;
        IsGameOver = true;
    }

    private static void SetLevel(int l)
    {
       Level = l;
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
            default:
                throw new Exception("unhandled mode");
        }

        // activate screens
        void ShowHide(GameObject g)
        {
            g.SetActive(g == activeScreen);
        }
        ShowHide(MainMenu);

        // update mode
        CurrentMode = mode;
    }

    public static void OnGameEvent(GameEvent e)
    {
        switch (CurrentMode)
        {
            case GameMode.MainMenu:
                if (e == GameEvent.StartButton)
                {
                    SetLevel(1);
                    ChangeMode(GameMode.Gameplay);
                }
                break;
            default:
                break;
        }
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return helperObject.StartCoroutine(routine);
    }


    private static readonly Dictionary<System.Type, MonoBehaviour> gameplayComponentsCache = new Dictionary<System.Type, MonoBehaviour>();
    public static T FindComp<T>() where T : MonoBehaviour
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
