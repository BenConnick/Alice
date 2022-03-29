using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EnterNameScreen : MonoBehaviour
{
    // serialized
    public RectTransform AlphabetArea;
    public RectTransform SelectedAlphabetCursor;
    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI AlphabetLabel;

    private const string alphabetString = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z <END>";
    private const int numNameCharacters = 5;
    private string enteredName = "";
    private const int alphabetRowLength = 10;
    private int alphabetX;
    private int selectedNameCharacter;

    public void OnEnable()
    {
        UpdateDataDrivenUIInstant();
    }

    public void Update()
    {
        UpdateDataDrivenUIInstant();
    }

    public void OnConfirmPressed()
    {
        // sanity check
        if (!gameObject.activeSelf) return;

        // confirm letter
        if (selectedNameCharacter < numNameCharacters - 1)
        {
            SetSelectedNameCharacter(selectedNameCharacter + 1);
        }
        // confirm name
        else
        {
            SubmitScoreName();
        }
    }

    private void SubmitScoreName()
    {
        // check for cheat codes
        foreach (var code in GM.CheatCodes)
        {
            if (enteredName == code.Key)
            {
                GM.PlayerHighScoreNames.Add(enteredName);
                GM.PlayerHighScores.Add(GM.CurrentScore);
                GM.OnCheatCode(code.Value);
                return;
            }
        }

        // not a cheat code
        GM.PlayerHighScoreNames.Add(enteredName);
        GM.PlayerHighScores.Add(GM.CurrentScore);
        GM.OnGameEvent(GM.NavigationEvent.CloseNamePicker);
    }

    public void UpdateDataDrivenUIInstant()
    {
        // assumes per-frame

        // update the name cursor
        char[] nameString = new char[numNameCharacters] { '_', '_', '_', '_', '_' };
        for (int i = 0; i < 5; i++)
        {
            if (i < enteredName.Length)
            {
                nameString[i] = enteredName[i];
            }
            else if (i == selectedNameCharacter)
            {
                nameString[i] = Time.time % 1 > 0.5f ? '_' : GetSelectedLetter();
            }
            else
            {
                if (i < enteredName.Length)
                {
                    nameString[i] = enteredName[i];
                }
                else
                {
                    nameString[i] = '_';
                }
            }
        }
        NameLabel.text = nameString.ArrayToString();

        // update the alphabet cursor
        string alphabetDisplayString = "";
        for (int i = 0; i < alphabetString.Length; i++)
        {
            if (alphabetX * 2 == i)
            {
                alphabetDisplayString += Time.time % 1 > 0.5f ? alphabetString[i] : '_';
            }
            else
            {
                alphabetDisplayString += alphabetString[i];
            }
        }
        AlphabetLabel.text = alphabetDisplayString;
    }

    private char GetSelectedLetter()
    {
        return alphabetString[alphabetX * 2];
    }

    private void OnVerticalDirectionPressed(int direction)
    {
        ChangeSelectedAlphabetLetter(0, direction);
    }

    private void OnHorizontalDirectionPressed(int direction)
    {
        ChangeSelectedAlphabetLetter(direction, 0);
    }

    public void OnUpPressed()
    {
        OnVerticalDirectionPressed(1);
    }

    public void OnDownPressed()
    {
        OnVerticalDirectionPressed(-1);
    }

    public void OnRightPressed()
    {
        OnHorizontalDirectionPressed(1);
    }

    public void OnLeftPressed()
    {
        OnHorizontalDirectionPressed(-1);
    }

    private void SetSelectedNameCharacter(int index)
    {
        selectedNameCharacter = index;
    }

    private void ChangeSelectedAlphabetLetter(int deltaX, int deltaY)
    {
        alphabetX += deltaX;
        alphabetX += alphabetRowLength * deltaY;

        // wrap around
        const int numLetters = 27;
        if (alphabetX < 0) alphabetX = numLetters - 1;
        if (alphabetX >= numLetters) alphabetX = 0;
    }
}
