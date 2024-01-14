using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesConfig", menuName = "ScriptableObjects/UpgradesConfig")]
public class UpgradesConfig : ScriptableObject
{
    public Upgrade[] AllUpgrades;

    public Upgrade Find(string name)
    {
        foreach (var u in AllUpgrades)
        {
            if (u.DisplayName == name) return u;
        }
        return default;
    }

    public Upgrade Find(UpgradeType upgrade)
    {
        foreach (var u in AllUpgrades)
        {
            if (u.Type == upgrade) return u;
        }
        return default;
    }
}

[Serializable]
public struct Upgrade
{
    public UpgradeType Type;
    public string DisplayName;
    public Sprite ShopSprite;
    public string ShopDescription;
    public int ShopCost;
}