//
// Copyright Imangi Studios, LLC, Copyright 2024. All rights reserved
//

using System;

[Serializable]
public class ScoreObject
{
    public int Coins;
    public int Meters;
    public int Hearts;

    public int MetersMultiplierValue => 1;
    public int CoinsMultiplierValue => 10;
    public int HeartsMultiplierValue => 100;
    
    public int GetCombinedScore()
    {
        return MetersMultiplierValue * Meters 
               + CoinsMultiplierValue * Coins 
               + HeartsMultiplierValue * Hearts;
    }

    public void Clear()
    {
        Coins = 0;
        Meters = 0;
        Hearts = 0;
    }
}
