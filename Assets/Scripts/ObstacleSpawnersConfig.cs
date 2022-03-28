using System;
using UnityEngine;

[CreateAssetMenu]
public class ObstacleSpawnersConfig : ScriptableObject
{
    [Header("Shared")]
    public int obstacleLaneMax; // total number of lanes, the min is zero
    [Header("RabbitHole")]
    public ObstacleSpawnCurve RabbitHoleCurve;
    [Header("Caterpillar")]
    public ObstacleSpawnCurve CaterpillarCurve;
}

[Serializable]
public struct ObstacleSpawnCurve
{
    public float secondsBetweenObstacles; // baseline seconds between obstacles. This isn't exact: the actual time between obstacles is an equation that uses the modifiers below
    public AnimationCurve difficultyCurve; // multiplier to time-between obstacles, > 0 means divide by this, < 0 means divide by the abs value
    public float difficultyCurveMagnitude; // percent of original; how big of a "swing"
    public float difficultyCurveDuration; // seconds
    public float difficultyAcceleration; // continuous per-second change in seconds between obstacles, positive means "gets faster over time", use a very very small number
    public float difficultyCap; // time between obstacles in seconds

    public float ComputeSpawnInterval(float elapsedTime)
    {
        // this is the baseline difficulty
        float baseline = secondsBetweenObstacles;

        // this is a multiplier on the baseline difficulty that makes the level get harder slowly over time.
        float continualModifier = 1 + difficultyAcceleration * elapsedTime;

        // this is a multiplier on the baseline difficulty that fluctuates the difficulty so that it's not so monotonous.
        float loopedTime = (elapsedTime % difficultyCurveDuration) / difficultyCurveDuration;
        float normalizedLooping = difficultyCurve.Evaluate(loopedTime);
        float loopingModifier = 1 - normalizedLooping * difficultyCurveMagnitude;

        // compute the result
        // apply the continual modifier first to get the potential
        float potential = baseline * continualModifier;
        // consider the looping modifier to be a percentage of the potential
        // e.g. 1.5 is 150% of potential, 0.25 is 25%, etc.
        float final = potential * loopingModifier;

        return Mathf.Max(difficultyCap, final);
    }
}