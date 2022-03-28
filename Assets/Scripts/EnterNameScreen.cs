using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnterNameScreen : MonoBehaviour
{
    // serialized
    public RectTransform AlphabetArea;
    public RectTransform SelectedAlphabetCursor;
    public TextMeshProUGUI NameLabel;

    private const int numNameCharacters = 5;
    private string enteredName;
    private int alphabetX, alphabetY;
    private int selectedNameCharacter;

    public void OnEnable()
    {
        UpdateDataDrivenUIInstant();
    }

    public void Update()
    {
        UpdateDataDrivenUIInstant();
    }

    public void UpdateDataDrivenUIInstant()
    {
        // assumes per-frame

        // update the name cursor
        string nameString = "_____";
        for (int i = 0; i < 5; i++)
        {
            if (i == selectedNameCharacter)
            {
                nameString[i] = Time.time % 1 > 0.5f ? "_" : " ";
            }
            else
            {
                if (i < enteredName.Length)
                {
                    nameString[i] = enteredName[i];
                }
                else
                {
                    nameString[i] = "_";
                }
            }
        }

        // update the alphabet cursor
        Rect r = AlphabetArea.rect;
        SelectedAlphabetCursor.transform.localPosition = new Vector2(
            r.width * alphabetX - r.width * .5f,
            r.height * alphabetY - r.height * .5f);
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
        alphabetY += deltaY;
    }
}
