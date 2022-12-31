using System;
using TMPro;
using UnityEngine;

[Obsolete]
public class CharacterSpeechOverlay : MonoBehaviour
{
    [SerializeField] private FadeUIBehavior fadeUIBehavior;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TypewriterText dialogueLabel;

    private CharacterDialogueSource source;

    public static void Show(CharacterDialogueSource source)
    {
        var self = GM.FindSingle<CharacterSpeechOverlay>();
        self.source = source;
        self.fadeUIBehavior.FadeInWithCallback(0.4f, () =>
        {
            self.ShowNextText();
        });
        self.nameLabel.text = source.GetName();
        self.dialogueLabel.Clear();
    }

    private void ShowNextText()
    {
        dialogueLabel.PlayTypewriter(source.GetNextLine());
    }

    public void OnNextPressed()
    {
        ShowNextText();
    }

    public void OnClosePressed()
    {
        fadeUIBehavior.FadeOutWithCallback(0.2f, () => {
            gameObject.SetActive(false);
        });
    }
}
