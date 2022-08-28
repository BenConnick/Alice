using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesConfig", menuName = "ScriptableObjects/UpgradesConfig")]
public class UpgradesConfig : ScriptableObject
{
    public UpgradeStats[] AllUpgrades;

    public UpgradeStats Find(string name)
    {
        foreach (var u in AllUpgrades)
        {
            if (u.Name == name) return u;
        }
        return default;
    }
}

[Serializable]
public struct UpgradeStats
{
    public string Name;
    public Sprite ShopSprite;
    public string ShopDescription;
    public int ShopCost;
}