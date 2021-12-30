using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

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
        ExploreLab, // the laboratory section
        LabCutscene,
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
            LivesChangedEvent.Invoke();

            // game over
            if (lives <= 0)
            {
                OnGameOverCondition();
            }
        }
    }
    public static int Level
    {
        get;
        private set;
    }
    public static bool IsGameOver { get; private set; }
    public static bool IsGameplayPaused { get; private set; }
    public static bool InputFrozen => IsGameplayPaused || IsGameOver;
    public static bool FellThroughFloor { get; set; }
    public static GameMode CurrentMode { get; private set; }
    public static int LabCutsceneIndex { get; private set; }

    // global references shortcuts
    //public static CameraController CameraController { get; private set; }

    public static GameObject MainMenu => helperObject.MainMenu;

    private static TextListAsset secretNotes;

    private static GMHelperObject helperObject;

    public interface IGameplayUI
    {

    }

    public static void Init(GMHelperObject helper)
    {
        if (helperObject != null || helper == null) return;

        helperObject = helper;

        IsGameOver = false;

        // collect references to scene objects
        // TODO

        // randoms
        //Random.InitState(PlatformManager.RandomSeed);

        // load save
        //PlatformerSaveData.Load();

        // load the dialogue data
        var linkToData = Resources.Load<TextAsset>("dialoguelink");
        string[] paths = linkToData.text.Split('\n');
        //TextPlatformLoader.Init(paths);

        // load the secret notes data
        secretNotes = Resources.Load<TextListAsset>("SecretNotes");

        // start the game
        // TODO start in the main menu mode
        // For now...
        // Start in "laboratory" mode
        ChangeMode(GameMode.MainMenu);

        // TODO start from to lab UI
        //PlatformManager.PlayLevel(Level);
    }

    public static void InitEditor()
    {
        // load the dialogue data
        var linkToData = Resources.Load<TextAsset>("dialoguelink");
        string[] paths = linkToData.text.Split('\n');

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
        ChangeMode(GameMode.ExploreLab);
    }

    private static void OnGameOverCondition()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        PlaySlowmoWithCallback(() =>
        {
            IsGameplayPaused = true;
            GameOverEvent.Invoke();
        });
    }

    public static void OnLevelUpCondition()
    {
        PlaySlowmoAndResume();
        // TODO GameplayUI.PlayShowCurtainAnimation(OnLevelUpAnimationComplete);
    }

    private static void OnLevelUpAnimationComplete()
    {
        IsGameplayPaused = true;
        SetLevel(Level + 1);
        LevelUpEvent.Invoke();
        // TODO GameplayUI.HideCurtain();
    }

    private static void SetLevel(int l)
    {
       Level = l;
       // PlatformManager.PlayLevel(Level);
       // CameraController.SetY(-PlatformManager.PrebakeDistance);
       // PC.transform.position = new Vector3(0, 0, 0);
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return helperObject.StartCoroutine(routine);
    }

    public static void SlowResume()
    {
        IsGameplayPaused = false;
        StartCoroutine(LerpTimescaleCoroutine(0.01f, 1f));
    }

    public static void PlaySlowmoAndResume()
    {
        StartCoroutine(ArcLerpTimescaleCoroutine(0.1f, 1f));
    }

    public static void PlaySlowmoWithCallback(Action callback)
    {
        StartCoroutine(LerpTimescaleCoroutine(0.1f, 0.01f, callback));
    }

    public static void JumpCameraToPlayer()
    {
        // TODO
    }

    private static float resumeStart;
    private const float resumeDuration = 1f;
    private static IEnumerator LerpTimescaleCoroutine(float initial, float final, Action callback = null)
    {
        resumeStart = Time.unscaledTime;
        while (Time.unscaledTime < resumeStart + resumeDuration) {
            float t = (Time.unscaledTime - resumeStart) / resumeDuration;
            t = t * t;
            Time.timeScale = Mathf.Lerp(initial, final, t);
            //Debug.Log("timescale " + Time.timeScale);
            yield return null;
        }
        Time.timeScale = 1;
        callback?.Invoke();
        //Debug.Log("timescale " + Time.timeScale);
    }

    private static IEnumerator ArcLerpTimescaleCoroutine(float middle, float final)
    {
        resumeStart = Time.unscaledTime;
        while (Time.unscaledTime < resumeStart + resumeDuration)
        {
            float t = (Time.unscaledTime - resumeStart) / resumeDuration;
            t = t * t - t + 0.25f;
            Time.timeScale = Mathf.Lerp(middle, final, t);
            //Debug.Log("timescale " + Time.timeScale);
            yield return null;
        }
        Time.timeScale = 1;
        //Debug.Log("timescale " + Time.timeScale);
    }

    public static event PlatformEventDelegate PlatformHitEvent;
    public static event PlatformEventDelegate PlatformLeftEvent;
    public static event EventDelegate LivesChangedEvent;
    public static event EventDelegate GameOverEvent;
    public static event EventDelegate LevelUpEvent;
    public static event NoteEventDelegate NoteCollectedEvent;

    public static void OnPlatformHit(int id)
    {
        PlatformHitEvent.Invoke(id);
    }

    public static void OnPlatformLeft(int id)
    {
        PlatformLeftEvent.Invoke(id);
    }

    public static void OnNoteCollected(int id)
    {
        NoteCollectedEvent.Invoke(id);
    }

    public static bool TryGetSecretNote(int note, out string text)
    {
        text = null;
        if (secretNotes == null) return false;
        return secretNotes.TryGet(note, out text);
    }

    [Command]
    public static void SkipToLevel2()
    {
        SetLevel(1);
    }

    public static void PlayRewind()
    {
        StartCoroutine(RewindCoroutine(resumeDuration));
    }

    private static IEnumerator RewindCoroutine(float duration)
    {
        float rewindStart = Time.unscaledTime;
        while (Time.unscaledTime < rewindStart + duration)
        {
            float t = (Time.unscaledTime - rewindStart) / duration;
            //CameraController.PlayRewindAnimation();
            // PC.transform.localPosition += new Vector3(0, 10*Time.unscaledDeltaTime, 0);
            yield return null;
        }
        // PC.SetVelocity(new Vector2(0, 10));

        // PlatformManager.UpdateSpeedZone(PC.transform.localPosition.y, PC.transform.localPosition.y, true);
        IsGameplayPaused = false;
    }

    public static void FallThroughFloor()
    {
        Lives = 0;
        FellThroughFloor = true;
        OnGameOverCondition();
    }

    private static void ChangeMode(GameMode mode)
    {
        GameObject activeScreen = null;
        switch (mode)
        {
            case GameMode.MainMenu:
                activeScreen = MainMenu;
                break;
            case GameMode.ExploreLab:
                //LabUI.ShowExplore();
                //activeScreen = LabUI.transform.parent.gameObject;
                break;
            case GameMode.LabCutscene:
                //LabUI.ShowCutscene();
                //activeScreen = LabUI.transform.parent.gameObject;
                break;
            case GameMode.Gameplay:
                //activeScreen = GameplayUI.transform.parent.gameObject;
                //PlatformManager.PlayLevel(Level); // play current level
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
        // TODO
        //ShowHide(LabUI.transform.parent.gameObject);
        //ShowHide(GameplayUI.transform.parent.gameObject);

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
                    ChangeMode(GameMode.LabCutscene);
                }
                else if (e == GameEvent.SkipIntroDialogue)
                {
                    ChangeMode(GameMode.ExploreLab);
                }
                else if (e == GameEvent.StartLevel)
                {
                    ChangeMode(GameMode.Gameplay);
                }
                break;
            case GameMode.LabCutscene:
                if (e == GameEvent.CutsceneEnded)
                {
                    ChangeMode(GameMode.ExploreLab);
                }
                break;
            case GameMode.ExploreLab:
                if (e == GameEvent.StartLevel)
                {
                    ChangeMode(GameMode.Gameplay);
                }
                break;
            default:
                break;
        }
    }
}
