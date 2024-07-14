using UnityEngine;

public class ScoreViewController : MonoBehaviour
{
    [SerializeField] 
    private TypewriterText distanceLabel;
    [SerializeField]
    private TypewriterText coinsLabel;
    [SerializeField]
    private TypewriterText heartsLabel;
    [SerializeField]
    private TypewriterText totalLabel;

    public void OnEnable()
    {
        FallingGameInstance game = ContextualInputSystem.ActiveGameInstance;
        ScoreObject scores = game.VpScore;
        distanceLabel.PlayTypewriter($"DEPTH:   {scores.MetersMultiplierValue * scores.Meters:00000}", delay: 0.5f);
        coinsLabel.PlayTypewriter($"RABBITS: {scores.CoinsMultiplierValue * scores.Coins:00000}", delay: 1f);
        heartsLabel.PlayTypewriter($"HEARTS:  {scores.HeartsMultiplierValue * scores.Hearts:00000}", delay: 1.5f);
        totalLabel.PlayTypewriter($"TOTAL: {scores.GetCombinedScore():00000}", delay: 2.5f);
    }
}