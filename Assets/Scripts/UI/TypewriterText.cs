using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueLabel;
    [SerializeField] private TextMeshPro dialogueTextMesh;
    [SerializeField] private AudioSource soundSource;

    [Range(0.0f, 1.0f)]
    public float ProgressNorm;

    [Header("Auto config")]
    [SerializeField] private string displayString;
    [SerializeField] private bool autoPlay;
    [SerializeField] private float defaultSpeed = 10;

    private struct InputData
    {
        public AudioClip[] TypewriterSounds;
        public float TypewriterSpeed;
    }

    // animation
    
    private InputData inputData;
    private int prevTextAppearingAnimationCharacter;
    public bool IsAutoPlaying => inputData.TypewriterSpeed > 0;

    public void PlayTypewriter(string text, float speed = 0, AudioClip[] sounds = null, float delay = 0f)
    {
        // invalid input
        if (text == null) return;

        // defaults
        if (sounds == null) sounds = new AudioClip[] { soundSource.clip };
        if (speed <= 0) speed = defaultSpeed;

        // params
        displayString = text;
        inputData = new InputData { TypewriterSounds = sounds, TypewriterSpeed = speed };

        // resetting
        VisualReset();

        // delay
        ProgressNorm -= (inputData.TypewriterSpeed * delay * displayString.Length); 
    }

    public void Clear()
    {
        inputData = default;
        VisualReset();
    }

    private void VisualReset()
    {
        // reset ui
        ProgressNorm = 0;
        SetText(""); // dialogueLabel.text handled by Update()
    }

    private void Start()
    {
        if (autoPlay)
        {
            PlayTypewriter(displayString);
        }
    }

    private void Update()
    {
        if (displayString == null) return;
        ProgressNorm += (Time.deltaTime * inputData.TypewriterSpeed) / displayString.Length;

        // update text
        UpdateText();
    }

    private void UpdateText()
    {
        string fullText = displayString;
        if (ProgressNorm < 1)
        {
            int character = Mathf.FloorToInt(ProgressNorm * fullText.Length);
            if (character < 0) character = 0;

            // play bleep for each character (if it's not whitespace)
            if (character < fullText.Length && character > prevTextAppearingAnimationCharacter && !char.IsWhiteSpace(fullText[character]))
            {
                AudioClip[] audioClips = inputData.TypewriterSounds;
                if (audioClips.Length > 0 && audioClips[0] != null)
                {
                    soundSource.PlayOneShot(audioClips[0]);
                }
            }
            prevTextAppearingAnimationCharacter = character;

            // show text one character at a time
            int split = Mathf.Min(character, fullText.Length);
            string visibleText = fullText.Substring(0, split);
            string invisibleText = fullText.Substring(split);
            SetText($"<alpha=#FF>{visibleText}<alpha=#00>{invisibleText}");
        }
        else
        {
            SetText($"<alpha=#FF>{fullText}<alpha=#00>");
        }
    }

    private void SetText(string text)
    {
        if (ReferenceEquals(dialogueLabel, null))
        {
            // TextMesh
            dialogueTextMesh.text = text;
        }
        else
        {
            // UGUI
            dialogueLabel.text = text;
        }
    }

    private string GetText()
    {
        if (ReferenceEquals(dialogueLabel, null))
        {
            // TextMesh
            return dialogueTextMesh.text;
        }
        else
        {
            // UGUI
            return dialogueLabel.text;
        }
    }

    public float GetProgress()
    {
        if (string.IsNullOrEmpty(displayString)) return 0;
        return ProgressNorm;
    }

    public void Finish()
    {
        ProgressNorm = 1;
    }

    //private void Experimental()
    //{
    //    int count = dialogueTextMesh.textInfo.characterCount;
    //    for (int i = 0; i < count; i++)
    //    {
    //        Vector3 bottom = dialogueTextMesh.textInfo.characterInfo[i].bottomLeft;
            
    //    }
    //    //
    //}
}
