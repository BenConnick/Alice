using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private FadeUIBehavior fader;
    [SerializeField] private TextMeshProUGUI upgradeHeaderLabel;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionLabel;
    [SerializeField] private TextMeshProUGUI buyLabel;
    [SerializeField] private TextMeshProUGUI cancelLabel;
    [SerializeField] private TextMeshProUGUI bankLabel;
    [SerializeField] private Image upgradeImage;

    private Upgrade upgradeToBuy;

    public void Show(string upgradeName)
    {
        var config = Resources.Load<UpgradesConfig>("UpgradesConfig");
        upgradeToBuy = config.Find(upgradeName);
        ShowInner(upgradeToBuy);
    }

    public void Show(UpgradeType upgradeType)
    {
        var config = Resources.Load<UpgradesConfig>("UpgradesConfig");
        upgradeToBuy = config.Find(upgradeType);
        ShowInner(upgradeToBuy);
    }

    private void ShowInner(Upgrade upgrade)
    {
        upgradeToBuy = upgrade; // cache for click event
        upgradeHeaderLabel.text = upgrade.DisplayName;
        upgradeDescriptionLabel.text = upgrade.ShopDescription;
        buyLabel.text = "Buy for " + upgrade.ShopCost + Util.CurrencyChar;
        bankLabel.text = "" + GM.Money + Util.CurrencyChar;
        upgradeImage.sprite = upgrade.ShopSprite;

        gameObject.SetActive(true);
        fader.FadeInWithCallback(0.6f, null);
        ContextualInputSystem.UICapturedInput = true;
    }

    public void OnBuyPressed()
    {
        Debug.Log("purchase " + upgradeToBuy.DisplayName + " TODO");
        Close();
    }

    public void OnCancelPressed()
    {
        Close();
    }

    private void Close()
    {
        ContextualInputSystem.UICapturedInput = false;
        fader.FadeOutWithCallback(0.4f, () => {
            gameObject.SetActive(false);
        });
    }
}
