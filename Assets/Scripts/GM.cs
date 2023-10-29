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

    public const string PassageStartPrefix = "Down the Rabbit Hole";
    public const string Caterpillar = "Conversation With A Caterpillar";
    public static int MAX_LIVES = 3;
    //public static MenuStage CurrentMenuStage { get; set; }
    public static LevelType CurrentLevel { get; set; } = LevelType.RabbitHole;

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

    public static float GetFallSpeed()
    {
        float baseline = helperObject.DefaultFallSpeed;

        // increase fall speed the farther down you are
        float depth = ContextualInputSystem.Context.GetProgressTotal();
        return baseline + helperObject.FallAcceleration * depth;
    }


    private static float GetLevelTimeScale(LevelType level)
    {
        return level == LevelType.Caterpillar ? .5f : 1f;
    }
    #endregion

    #region game event handlers
    public enum NavigationEvent
    {
        Default,
        MainMenuGoNext,
        PlatformerGameOver,
        PlatformerLevelEndTrigger,
        PlatformerLevelEndPostAnimation,
        FallFromMonologue,
        SplitAnimationMidPoint,
        GameOverGoNext,
        DialogueGoNext,
        BedInteraction,
        MenuAnimationFinished
    }

    public static void OnGameEvent(NavigationEvent gameEvent)
    {
        switch (gameEvent)
        {
            case NavigationEvent.MainMenuGoNext:
                {
                    FindSingle<Alice>().UnbecomeButton();
                    BeginGameplayMode();
                }
                break;
            case NavigationEvent.PlatformerGameOver:
                {
                    CurrentMode = GameMode.GameOver;
                    DeathCount++;
                    //string storyKey = PassageStartPrefix + (DeathCount < 2 ? "" : " " + DeathCount);
                    //ShowStory(storyKey);

                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(true);
                    });
                }

                break;
            case NavigationEvent.PlatformerLevelEndTrigger:
                {
                    // play outro
                    AllDisplays(rhd => rhd.ObstacleContext.PlayOutroAnimation());
                }
                break;
            case NavigationEvent.PlatformerLevelEndPostAnimation:
                {
                    // begin dialogue
                    CurrentMode = GameMode.Dialogue;
                    CurrentLevel++;
                    FindSingle<Alice>().BecomeButton();
                    AdvanceDialogueContext();
                }
                break;
            case NavigationEvent.FallFromMonologue:
                {
                    AllDisplays(rhd =>
                    {
                        var rh = rhd.ObstacleContext;
                        rh.Reset();
                        rh.PlayIntroAnimationForRestart();
                    });
                    FindSingle<GameplayScreenBehavior>().ShowGame();
                }
                break;
            case NavigationEvent.SplitAnimationMidPoint:
                {
                    BeginGameplayMode();
                }
                break;
            case NavigationEvent.GameOverGoNext:
                {
                    Debug.Log("GameOverGoNext");
                    CurrentMode = GameMode.Gameplay;

                    // reset all displays
                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(false);
                        var rh = disp.ObstacleContext;
                        rh.Reset();
                        rh.menuGraphics.ShowStageArt(CurrentLevel);
                        rh.PlayTitleIntro();
                    });

                    // set mode to main menu
                    CurrentMode = GameMode.PreMainMenu;

                    // set alice position
                    var alice = FindSingle<Alice>();
                    var ctx = alice.movementContext;
                    if (ctx != null)
                    {
                        Vector3 center = ctx.GetCursorViewportWorldPos(new Vector2(.5f, .5f));
                        alice.transform.position = center + new Vector3(2.5f, -2.5f, 0);
                        alice.BecomeButton();
                    }
                }
                break;
            case NavigationEvent.DialogueGoNext:
                {
                    AdvanceDialogueContext();
                }
                break;
            case NavigationEvent.BedInteraction:
                {
                    Debug.Log("BedInteraction");
                    CurrentMode = GameMode.Gameplay;
                    AllDisplays(disp =>
                    {
                        GameObject gameOverUI = disp.GameplayGroup.UIOverlay.GameOverOverlay;
                        gameOverUI.SetActive(false);
                        var rh = disp.ObstacleContext;
                        rh.Reset();
                        rh.PlayIntroAnimationForRestart();
                    });
                }
                break;
            case NavigationEvent.MenuAnimationFinished:
                {
                    Debug.Log("MenuAnimationFinished");
                    CurrentMode = GameMode.MainMenu;
                }
                break;
        }
    }

    private static void AllDisplays(Action<RabbitHoleDisplay> dispAction)
    {
        foreach (var disp in RabbitHoleDisplay.All)
        {
            dispAction(disp);
        }
    }

    private static void BeginGameplayMode()
    {
        CurrentMode = GameMode.Gameplay;
        FindSingle<Alice>().UnbecomeButton();
        TimeDistortionController.SetBaselineSpeed(GetLevelTimeScale(CurrentLevel));
        foreach (var disp in RabbitHoleDisplay.All)
        {
            var hole = disp.ObstacleContext;
            hole.Reset();
            hole.PlayIntroAnimationForCurrentLevel();
        }
    }

    private static void PlayCaterpillarDoneMoment()
    {
        FindSingle<SplitGameplayMomentAnimationController>().RevealSecondView();
    }

    private static void AdvanceDialogueContext()
    {
        // get current dialogue context
        var ctx = CharacterDialogueBehavior.ActiveDialogue;

        if (ctx == null)
        {
            Debug.LogError("Dialogue advanced, but no dialogue active");
            return;
        }

        // advance to next step
        bool last = ctx.PlayNextLine();
        // if this is the last step, start the next falling section
        if (last)
        {
            if (ctx is CaterpillarDialogueBehavior)
                PlayCaterpillarDoneMoment();
            else
                BeginGameplayMode();
        }
    }

    /*private static void ShowStory(string storyKey)
    {
        FindSingle<GameplayScreenBehavior>().ShowStory(storyKey);
        CurrentMode = GameMode.Dialogue;
    }*/
    #endregion
}
