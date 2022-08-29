using System.Collections;
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
        var config = Resources.Load<UpgradesConfig>("UpgradesConfig");
        var upgrade = config.Find(upgradeName);
        ShowInner(upgrade);
    }

    public void Show(UpgradeType upgradeType)
    {
        var config = Resources.Load<UpgradesConfig>("UpgradesConfig");
        var upgrade = config.Find(upgradeType);
        ShowInner(upgrade);
    }

    private void ShowInner(Upgrade upgrade)
    {
        upgradeHeaderLabel.text = upgrade.DisplayName;
        upgradeDescriptionLabel.text = upgrade.ShopDescription;
        buyLabel.text = "Buy for " + upgrade.ShopCost + Util.CurrencyChar;
        bankLabel.text = "TODO" + Util.CurrencyChar;
        upgradeImage.sprite = upgrade.ShopSprite;

        gameObject.SetActive(true);
        StartCoroutine(FadeIn(.75f));
    }

    private IEnumerator FadeIn(float duration)
    {
        if (duration == 0) duration = 1f;
        canvasGroup.alpha = 0;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = t;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 1;
    }
}
