using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameOverTextAnimationBehavior : MonoBehaviour
{
    public string Text1 = "GAME OVER";
    public string Text2 = "Play Me";
    public float SecondsUntilChange = .5f;
    public bool PlayOnEnable = true;

    private TextMeshProUGUI label;

    private void OnEnable()
    {
        label = GetComponent<TextMeshProUGUI>();
        if (PlayOnEnable) Play();
    }

    public void Play()
    {
        StartCoroutine(AnimateTextContentOneShot());
    }

    private IEnumerator AnimateTextContentOneShot()
    {
        // set first
        label.text = Text1;

        // wait seconds
        float startTime = Time.time;
        while (Time.time - startTime < SecondsUntilChange)
        {
            yield return null;
        }

        // set second
        label.text = Text2;
    }
}
