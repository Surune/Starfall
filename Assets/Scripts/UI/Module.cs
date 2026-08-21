using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Audio;
using Gameplay.Managers;

namespace UI
{
public class Module : MonoBehaviour
{
    SoundManager Sound => GameManager.Instance.SoundManager;
    
    [SerializeField] int type;
    [SerializeField] TMP_Text priceText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text descriptionText;
    public Button button;
    private Store store;
    private int price;
    private int level;

    static readonly string[] ModuleTextList = {"공격력\n", "치명타 확률\n", "치명타 대미지\n", "새로고침\n", "시작 경험치\n", "코인 획득량\n", "적 체력\n", "적 속도\n"};
    static readonly int[] MaxLevelList = {100, 100, 100, 3, 10, 50, 20, 20};
    static readonly int[] PriceList = {10, 10, 10, 100, 30, 20, 25, 25};

    private void Start()
    {
        level = PlayerPrefs.GetInt("module_" + type);
        store = GetComponentInParent<Store>();
        SetLevelText();
        SetPriceText();
        SetDescriptionText();
        button.onClick.AddListener(UpgradeModule);
    }

    private void SetLevelText()
    {
        levelText.text = $"LV. {level}/{MaxLevelList[type-1]}";
    }

    private void SetPriceText()
    {
        if (level >= MaxLevelList[type-1])
        {
            button.interactable = false;
            priceText.text = "MAX";
        }
        else
        {
            price = (level + 1) * PriceList[type-1];
            priceText.text = price.ToString();
            if (price > PlayerPrefs.GetInt("TotalCoin", 0))
            {
                priceText.color = Color.red;
                button.interactable = false;
            }
            else
            {
                priceText.color = Color.black;
                button.interactable = true;
            }
        }
    }

    private void SetDescriptionText()
    {
        switch (type)
        {
            case 1:
                descriptionText.text = $"{ModuleTextList[type-1]}+{0.02 * level}";
                break;
            case 2:
            case 3:
            case 6:
                descriptionText.text = $"{ModuleTextList[type-1]}+{0.5 * level}%";
                break;
            case 4:
            case 5:
                descriptionText.text = $"{ModuleTextList[type-1]}+{level}";
                break;
            case 7:
                descriptionText.text = $"{ModuleTextList[type-1]}-{0.05 * level}";
                break;
            case 8:
                descriptionText.text = $"{ModuleTextList[type-1]}-{0.5 * level})%";
                break;
        }
    }

    public void UpgradeModule()
    {
        var coins = PlayerPrefs.GetInt("TotalCoin", 0);
        if (coins >= price && level < MaxLevelList[type-1])
        {
            coins -= price;
            level += 1;
            PlayerPrefs.SetInt($"module_{type}", level);
            PlayerPrefs.SetInt("TotalCoin", coins);
            SetLevelText();
            SetPriceText();
            SetDescriptionText();

            foreach (var module in FindObjectsOfType<Module>())
            {
                module.SetPriceText();
            }
            store.SetCoins(coins);
            Sound.PlaySFX(SoundKey.Upgrade);
        }
        else
        {
            Sound.PlaySFX(SoundKey.Fail);
        }
    }
}
}
