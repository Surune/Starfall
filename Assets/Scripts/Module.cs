using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;
using UnityEngine.Serialization;

public class Module : MonoBehaviour
{
    static SFXManager SfxManager => GameManager.Instance.SfxManager;
    [SerializeField] int type;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI descriptionText;
    public Button button;
    TextMeshProUGUI CoinText;
    int price;
    int level;

    static readonly string[] ModuleTextList = {"공격력\n", "치명타 확률\n", "치명타 대미지\n", "새로고침\n", "시작 경험치\n", "코인 획득량\n", "적 체력\n", "적 속도\n"};
    static readonly int[] MaxLevelList = {100, 100, 100, 3, 10, 50, 20, 20};
    static readonly int[] PriceList = {10, 10, 10, 100, 30, 20, 25, 25};

    void Start()
    {
        level = PlayerPrefs.GetInt("module_" + type);
        CoinText = GameObject.Find("Coin").GetComponent<TextMeshProUGUI>();
        SetLevelText();
        SetPriceText();
        SetDescriptionText();
        button.onClick.AddListener(UpgradeModule);
    }

    public void SetLevelText()
    {
        levelText.text = $"LV. {level}/{MaxLevelList[type-1]}";
    }

    public void SetPriceText()
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

    public void SetDescriptionText()
    {
        if (type == 1)
        {
            descriptionText.text = $"{ModuleTextList[type-1]}+{0.02 * level}";
        }
        else if (type == 2 || type == 3 || type == 6)
        {
            descriptionText.text = $"{ModuleTextList[type-1]}+{0.5 * level}%";
        }
        else if (type == 4 || type == 5)
        {
            descriptionText.text = $"{ModuleTextList[type-1]}+{level}";
        }
        else if (type == 7)
        {
            descriptionText.text = $"{ModuleTextList[type-1]}-{0.05 * level}";
        }
        else if (type == 8)
        {
            descriptionText.text = $"{ModuleTextList[type-1]}-{0.5 * level})%";
        }
    }

    public void UpgradeModule()
    {
        int coins = PlayerPrefs.GetInt("TotalCoin", 0);
        if (coins >= price && level < MaxLevelList[type-1])
        {
            coins -= price;
            level += 1;
            PlayerPrefs.SetInt($"module_{type}", level);
            PlayerPrefs.SetInt("TotalCoin", coins);
            SetLevelText();
            SetPriceText();
            SetDescriptionText();

            foreach (Module module in FindObjectsOfType<Module>())
            {
                module.SetPriceText();
            }
            CoinText.text = $"Coins : {coins}";
            SfxManager.PlayUpgrade();
        }
        else
        {
            SfxManager.PlayFail();
        }
    }
}
