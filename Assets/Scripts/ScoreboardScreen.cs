using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

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

    private void GetScores(out int[] scoresToUse, out string[] namesToUse)
    {
        // default arguments
        MixScores(defaultScores, GM.PlayerHighScores, defaultScoreNames, GM.PlayerHighScoreNames,
            out scoresToUse, out namesToUse);
    }

    public void UpdateDataDrivenUIInstant()
    {
        // get the scores
        GetScores(out int[] scoresToUse, out string[] namesToUse);

        // build the string
        string scoresString = "";
        string scoreNamesString = "";
        for (int i = 0; i < scoresToUse.Length; i++)
        {
            int score = scoresToUse[i];
            string name = namesToUse[i];
            scoresString += "\n" + score;
            scoreNamesString += "\n" + name;
        }

        // current
        CurrentScoreLabel.text = "Your score: " + GM.CurrentScore;
        // high scores
        ScoresLabel.text = scoresString;
        ScoreNamesLabel.text = scoreNamesString;
    }

    public void MixScores(int[] builtInScores, List<int> playerScores, string[] builtInNames, List<string> playerNames, out int[] resultingScores, out string[] resultingNames)
    {
        resultingScores = new int[builtInScores.Length];
        resultingNames = new string[builtInScores.Length];
        for (int i = 0; i < builtInScores.Length; i++)
        {
            int score = builtInScores[i];
            string name = builtInNames[i];
            for (int j = 0; j < playerScores.Count; j++)
            {
                int playerScore = playerScores[j];
                if (playerScore > score)
                {
                    score = playerScore;
                    name = playerNames[j]; // assumed same length as scores
                }
            }
            resultingScores[i] = score;
            resultingNames[i] = name;
        }
    }

    public void OnContinuePressed()
    {
        // get the scores
        GetScores(out int[] scoresToUse, out string[] namesToUse);
        for (int i = 0; i < scoresToUse.Length; i++)
        {
            if (!GM.PlayerHighScores.Contains(GM.CurrentScore) && GM.CurrentScore > scoresToUse[i])
            {
                //GM.OnGameEvent(GM.NavigationEvent.OpenNamePicker);
                return;
            }
        }
        //GM.OnGameEvent(GM.NavigationEvent.CloseScoreboard);
    }
}
