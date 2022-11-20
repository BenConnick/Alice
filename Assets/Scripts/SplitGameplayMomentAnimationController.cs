using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitGameplayMomentAnimationController : MonoBehaviour
{
    [SerializeField] private RectTransform RabbitHoleDisplay1;
    [SerializeField] private RectTransform RabbitHoleDisplay2;
    [SerializeField] private GameObject RabbitHoleGroup2;
    [SerializeField] private RabbitHoleDisplay SecondRabbitHoleDisplay;
    [SerializeField] private RectTransform PlayerCharacterAnimDummy;
    [SerializeField] private GameObject ActualPlayerCharacter;
    [SerializeField] private GameObject[] MouseOverTriggers;

    [Header("Animation Config")]
    [SerializeField] private Vector2 dummyCanvasStartPos;
    [SerializeField] private float displayWidth=180;
    [SerializeField] private float gap = 100;
    [SerializeField] private float duration=1;

    // [Header("Debug")]
    // [SerializeField] private bool loopWhenFinished;

    public void SetToDefaultState()
    {
        // initialPositions
        RabbitHoleDisplay1.localPosition = Vector3.zero;
        RabbitHoleDisplay2.localPosition = Vector3.zero;

        // hide 2nd rabbit hole
        RabbitHoleDisplay2.gameObject.SetActive(false);
        RabbitHoleGroup2.SetActive(false);
    }

    // the 2nd gameplay frame activates and both gameplay viewports
    // slide to the side to make room for two on the screen (landscape)
    // display 1 goes left, revealing display 2, which goes right
    // in this first part of the animation, they leave a gap to show the
    // player character. This is not the normal character, but rather a special
    // on for the next part of the animation (described below)
    public void PlayBigMomentAnimPart1()
    {
        // hide the real player character
        ActualPlayerCharacter.SetActive(false);
        // show the dummy
        PlayerCharacterAnimDummy.gameObject.SetActive(true);
        // position the dummy
        PlayerCharacterAnimDummy.localPosition = dummyCanvasStartPos;
        // enable the mouse trigger areas
        foreach (var g in MouseOverTriggers) g.SetActive(true);

        // show the 2nd gameplay fame
        RabbitHoleDisplay2.gameObject.SetActive(true);
        RabbitHoleGroup2.SetActive(true);
        Tween.Start((t) =>
        {
            // interpolate
            float a = t; // easing
            float w = displayWidth + gap;
            RabbitHoleDisplay1.transform.localPosition = new Vector3(-w * a, 0, 0);
            RabbitHoleDisplay2.transform.localPosition = new Vector3(w * a, 0, 0);

            // force the cursor to the center of the screen
            // this will cause it to align with the dummy character
            // which is key for the next part, where the player moves it
            Cursor.lockState = CursorLockMode.Locked;
        },
        duration,
        () =>
        {
            // on complete
            Cursor.lockState = CursorLockMode.None;
        });
    }

    public void PlayBigMomentAnimPart2()
    {
        RabbitHoleDisplay2.gameObject.SetActive(true);
        Tween.Start((t) =>
        {
            // interpolate
            float a = Mathf.Log(t); // easing
            float wStart = displayWidth + gap;
            float wEnd = displayWidth;
            float lerp = Mathf.Lerp(wStart, wEnd, a);
            RabbitHoleDisplay1.transform.localPosition = new Vector3(-lerp, 0, 0);
            RabbitHoleDisplay2.transform.localPosition = new Vector3(lerp, 0, 0);
        },
        duration,
        () =>
        {
            // on complete
            var holes = FindObjectsOfType<RabbitHole>();
            foreach (var hole in holes)
            {
                // start both ? // TODO
                hole.PlayIntroAnimationForCurrentLevel();
            }
        });
    }

    public void OnMouseOverTrigger(int index)
    {
        foreach (var g in MouseOverTriggers) g.SetActive(false);
        PlayerCharacterAnimDummy.gameObject.SetActive(false);
        ActualPlayerCharacter.SetActive(true);
        PlayBigMomentAnimPart2();
    }
}
