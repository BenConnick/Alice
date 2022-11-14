using System;
using UnityEngine;

[Obsolete]
public class CharacterDialogueSource : MonoBehaviour
{
    //public
    public LevelType CharacterForLevel;
    public string OverrideName;

    [Header("Use only one of these")]
    public TextListAsset DialogueSource;
    public string[] DialogueLines;

    int index = -1;

    public void ShowDialogue()
    {
        CharacterSpeechOverlay.Show(this);
    }

    public string GetName()
    {
        return string.IsNullOrEmpty(OverrideName) ? GetCharacterDisplayName(CharacterForLevel) : OverrideName;
    }

    public string GetNextLine()
    {
        // advance
        index++;
        // wrap
        if (index >= GetLineCount()) index = 0;

        if (DialogueSource != null)
        {
            if (DialogueSource.Entries.Length == 0) return "Error";
            return DialogueSource.Entries[index];
        }
        else
        {
            if (DialogueLines.Length == 0) return "Error";
            return DialogueLines[index];
        }
    }

    public int GetLineCount()
    {
        if (DialogueSource != null)
        {
            return DialogueSource.Entries.Length;
        }
        else
        {
            return DialogueLines.Length;
        }
    }

    public static string GetCharacterDisplayName(LevelType levelType)
    {
        switch (levelType)
        {
            case LevelType.Default:
                return "Default";
            case LevelType.RabbitHole:
                return "Mouse";
            case LevelType.Caterpillar:
                return "Bug";
            case LevelType.CheshireCat:
                return "Cat";
            case LevelType.MadHatter:
                return "Hatta";
            case LevelType.QueenOfHearts:
                return "Queen of Blood";
            default:
                return "Error";
        }
    }
}
