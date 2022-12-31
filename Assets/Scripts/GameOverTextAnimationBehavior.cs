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

    // Start is called before the first frame update
    private void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (PlayOnEnable) Play();
    }

    public void Play()
    {
        StartCoroutine(AnimateTextContentOneShot());
    }

    private IEnumerator AnimateTextContentOneShot()
    {
        label.text = Text1;
        float startTime = Time.time;
        while (Time.time - startTime < SecondsUntilChange)
        {
            yield return null;
        }
        label.text = Text2;
    }
}
