#pragma warning disable CS0414

using UnityEngine;
using UnityEngine.UI;

public class SplitGameplayMomentAnimationController : MonoBehaviour
{
    [Header("Serialized References")]
    [SerializeField] private RectTransform RabbitHoleDisplay1;
    [SerializeField] private Transform playerCharacterTransform;
    [SerializeField] private RawImage display1Renderer;
    [SerializeField] private Animator animator;

    // inspector
    [Header("Animation Config")]
    [SerializeField] private float shakeDistance;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private Vector2 display1StartPos;
    [SerializeField] private Vector2 display2StartPos;
    [SerializeField] private Vector2 dummyCanvasStartPos;
    [SerializeField] private float displaySeparation;
    [SerializeField] private float playerEndWalkX;
    [SerializeField] private float walkDuration = 1;
    [SerializeField] private float spreadDuration = 1;
    [SerializeField] private float postWalkPauseDuration = 1;
    [SerializeField] private float fadeColorDuration = 1;

    // [Header("Debug")]
    // [SerializeField] private bool loopWhenFinished;

    public void SetToDefaultState()
    {
        // initialPositions
        RabbitHoleDisplay1.localPosition = display1StartPos;
    }

    // the 2nd gameplay frame activates and both gameplay viewports
    // slide to the side to make room for two on the screen (landscape)
    // display 1 goes left, revealing display 2, which goes right
    // in this first part of the animation, they leave a gap to show the
    // player character. This is not the normal character, but rather a special
    // on for the next part of the animation (described below)
    public void RevealSecondView()
    {
        // play animation: walk to shroom
        Root.Find<AliceCharacter>().GetComponent<SpriteAnimator>().SetAnimation("walking"); // walk
        Tween.Start(WalkToShroomInterpolate, walkDuration, OnWalkToShroomComplete);
    }

    private void WalkToShroomInterpolate(float t)
    {
        Vector3 pos = playerCharacterTransform.position;
        playerCharacterTransform.position = new Vector3(t * playerEndWalkX, pos.y, pos.z);
    }

    private void OnWalkToShroomComplete()
    {
        Root.Find<AliceCharacter>().GetComponent<SpriteAnimator>().SetAnimation("standing"); // walk
        Tween.StartDelayedAction(postWalkPauseDuration, OnPostWalkPauseComplete);
    }

    public void OnPostWalkPauseComplete()
    {
        // next animation: shake
        //Tween.Start(ShakeInterpolate, shakeDuration, OnShakeFinish);
        OnShakeFinish();
    }

    private void ShakeInterpolate(float t)
    {
        // shake replaced with Animator
    }

    private void OnShakeFinish()
    {
        // begin the fall
        Nav.Go(NavigationEvent.SplitAnimationMidPoint);

        // shake finished, separate
        //Tween.Start(SeparateInterpolate,spreadDuration,OnSeparateComplete);
        //Tween.Start(FadeInterpolate, fadeColorDuration);
        animator.SetTrigger("Wobble");
        OnSeparateComplete();
    }

    private void SeparateInterpolate(float t)
    {
        // [Deprecated]

        // interpolate (separate)
        //float a = t*t; // easing
        //float w = displaySeparation;
        //RabbitHoleDisplay1.transform.localPosition = new Vector3(-w * a, 0, 0);
        //RabbitHoleDisplay2.transform.localPosition = new Vector3(w * a, 0, 0);

        
        // force the cursor to the center of the screen
        // this will cause it to align with the dummy character
        // which is key for the next part, where the player moves it
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnSeparateComplete()
    {
        // unused
    }

    private void FadeInterpolate(float t)
    {
        //display1Renderer.color = new Color(t, 1, 1);
        //display2Renderer.color = new Color(1, 1, t);
    }

    //public void PlayBigMomentAnimPart2()
    //{
    //    foreach (var g in MouseOverTriggers) g.SetActive(false);
    //    PlayerCharacterAnimDummy.gameObject.SetActive(false);
    //    ActualPlayerCharacter.SetActive(true);
    //    RabbitHoleDisplay2.gameObject.SetActive(true);
    //    Tween.Start((t) =>
    //    {
    //        // interpolate
    //        float a = Mathf.Log(t); // easing
    //        float wStart = displayWidth + gap;
    //        float wEnd = displayWidth;
    //        float lerp = Mathf.Lerp(wStart, wEnd, a);
    //        RabbitHoleDisplay1.transform.localPosition = new Vector3(-lerp, 0, 0);
    //        RabbitHoleDisplay2.transform.localPosition = new Vector3(lerp, 0, 0);
    //    },
    //    duration,
    //    () =>
    //    {
    //        // on complete
    //        var holes = FindObjectsOfType<RabbitHole>();
    //        foreach (var hole in holes)
    //        {
    //            // start both ? // TODO
    //            hole.PlayIntroAnimationForCurrentLevel();
    //        }
    //    });
    //}

    // [Deprecated]
    //// hide the real player character
    //ActualPlayerCharacter.SetActive(false);
    //// show the dummy
    //PlayerCharacterAnimDummy.gameObject.SetActive(true);
    //// position the dummy
    //PlayerCharacterAnimDummy.localPosition = dummyCanvasStartPos;
    //// enable the mouse trigger areas
    //foreach (var g in MouseOverTriggers) g.SetActive(true);

    private void Update()
    {
        
    }
}
