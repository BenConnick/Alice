using UnityEngine;
using TMPro;

public class EnterNameScreen : MonoBehaviour
{
    // serialized
    public RectTransform AlphabetArea;
    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI AlphabetLabel;
    public GameObject[] letterRabbits;

    // constants
    private const string alphabetString = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z <END>";
    private const int numNameCharacters = 5;
    private const int alphabetRowLength = 10;

    // state
    private string enteredName = "";
    private int alphabetX;
    private int nameCursorIndex;

    public void OnEnable()
    {
        // reset
        enteredName = string.Empty;
        nameCursorIndex = 0;
        alphabetX = 0;
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
        if (nameCursorIndex < numNameCharacters - 1)
        {
            enteredName += GetSelectedLetter();
            SetNameCursorIndex(nameCursorIndex + 1);
        }
        // confirm name
        else
        {
            enteredName += GetSelectedLetter();
            SubmitScoreName();
        }
    }

    public void OnBackspacePressed()
    {
        if (enteredName.Length > 0)
        {
            enteredName = enteredName.Substring(0, enteredName.Length - 1);
            SetNameCursorIndex(nameCursorIndex - 1);
        }
    }

    private void SubmitScoreName()
    {
        // check for cheat codes
        foreach (var code in GM.CheatCodes)
        {
            if (enteredName == code.Value)
            {
                GM.PlayerHighScoreNames.Add(enteredName);
                GM.PlayerHighScores.Add(GM.CurrentScore);
                GM.OnCheatCode(code.Key);
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
            else if (i == nameCursorIndex)
            {
                nameString[i] = Time.time % 1 > 0.5f ? '_' : GetSelectedLetter();
            }
            else
            {
                nameString[i] = '_';
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

        // cheat-code UI
        // highlight the next letter in the "cheat" code
        int cheatHintIndex = -2;
        if (GM.CheatCodes.TryGetValue(1+GM.CurrentLevel, out string cheatCode))
        {
            // if the code can still be entered
            if (enteredName.Length == 0 || cheatCode.Contains(enteredName))
            {
                // if the code isn't fully entered
                if (enteredName.Length < cheatCode.Length)
                {
                    // highlight the next letter
                    cheatHintIndex = alphabetString.IndexOf(cheatCode[enteredName.Length]) / 2;
                }
                // if the code is fully entered
                else
                {
                    // highlight "DONE"
                    cheatHintIndex = alphabetString.IndexOf("END") / 2;
                }
            }
        }
        // hide all the rabbits except the one on the letter we want
        // rabbits are placed manually
        for (int i = 0; i < letterRabbits.Length; i++)
        {
            letterRabbits[i].SetActive(i == cheatHintIndex);
        }
    }

    private char GetSelectedLetter()
    {
        return alphabetString[alphabetX * 2];
    }

    private void OnHorizontalDirectionPressed(int direction)
    {
        ChangeSelectedAlphabetLetter(direction, 0);
    }

    public void OnRightPressed()
    {
        OnHorizontalDirectionPressed(1);
    }

    public void OnLeftPressed()
    {
        OnHorizontalDirectionPressed(-1);
    }

    private void SetNameCursorIndex(int index)
    {
        nameCursorIndex = index;
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
