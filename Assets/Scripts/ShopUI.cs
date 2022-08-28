using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI upgradeHeaderLabel;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionLabel;
    [SerializeField] private TextMeshProUGUI buyLabel;
    [SerializeField] private TextMeshProUGUI cancelLabel;
    [SerializeField] private TextMeshProUGUI bankLabel;
    [SerializeField] private Image upgradeImage;

    public void Show(string upgradeName)
    {
        var config = FindObjectOfType<UpgradesConfig>();
        upgradeHeaderLabel.text = upgradeName;
        var upgrade = config.Find(upgradeName);
        upgradeHeaderLabel.text = upgrade.Name;
        upgradeDescriptionLabel.text = upgrade.ShopDescription;
        buyLabel.text = "Buy for " + upgrade.ShopCost + Util.CurrencyChar;
        bankLabel.text = "TODO" + Util.CurrencyChar;
        upgradeImage.sprite = upgrade.ShopSprite;
    }
}
