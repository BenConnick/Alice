using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreboardScreen : MonoBehaviour
{
    public TextMeshProUGUI ScoreNamesLabel;
    public TextMeshProUGUI ScoresLabel;

    public TextMeshProUGUI CurrentScoreLabel;

    private readonly string[] defaultScoreNames = new string[] {
        "QUEEN", "TDNTD", "HATTA", "CHESH", "ABSOL", "WHITE"
    };
    private readonly int[] defaultScores = new int[] {
        1000, 800, 600, 400, 200, 10
    };

    public void OnEnable()
    {
        UpdateDataDrivenUIInstant();
    }

    public void UpdateDataDrivenUIInstant()
    {
        int[] scoresToUse = defaultScores;
        string[] namesToUse = defaultScoreNames; // assumed same length as scores^

        string scoresString = "";
        string scoreNamesString = "";
        for (int i = 0; i < scoresToUse.Length; i++)
        {
            int score = scoresToUse[i];
            string name = namesToUse[i];
            for (int j = 0; j < GM.PlayerHighScores.Count; j++)
            {
                int playerScore = GM.PlayerHighScores[j];
                if (playerScore > score)
                {
                    score = playerScore;
                    name = GM.PlayerHighScoreNames[j]; // assumed same length as scores
                }
            }
            scoresString += "\n" + score;
            scoreNamesString += "\n" + name;
        }

        // current
        CurrentScoreLabel.text = "Your score: " + GM.CurrentScore;
        // high scores
        ScoresLabel.text = scoresString;
        ScoreNamesLabel.text = scoreNamesString;
    }

    public void OnContinuePressed()
    {
        int[] scoresToUse = defaultScores;
        for (int i = 0; i < scoresToUse.Length; i++)
        {
            if (GM.CurrentScore > scoresToUse[i])
            {
                GM.OnGameEvent(GM.NavigationEvent.OpenNamePicker);
                return;
            }
        }
        GM.OnGameEvent(GM.NavigationEvent.CloseScoreboard);
    }
}
