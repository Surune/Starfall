using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;

public class Module : MonoBehaviour
{
    [SerializeField] int _type;
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] TextMeshProUGUI _levelText;
    public Button button;
    [SerializeField] TextMeshProUGUI _annaeText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    AudioSource MusicPlayer => GameManager.Instance.Player.MusicPlayer;
    [SerializeField] AudioClip _sfxUpgrade;
    [SerializeField] AudioClip _sfxFail;
    TextMeshProUGUI CoinText;
    int price;
    int level;

    static readonly string[] ModuleTextList = {"공격력\n", "치명타 확률\n", "치명타 대미지\n", "새로고침\n", "시작 경험치\n", "코인 획득량\n", "적 체력\n", "적 속도\n"};
    static readonly int[] MaxLevelList = {50, 50, 50, 5, 10, 50, 20, 20};
    static readonly int[] PriceList = {10, 10, 10, 100, 30, 20, 25, 25};

    void Start()
    {
        level = PlayerPrefs.GetInt("module_" + _type);
        CoinText = GameObject.Find("Coin").GetComponent<TextMeshProUGUI>();
        SetLevelText();
        SetPriceText();
        SetDescriptionText();
        SetAnnaeText("", new Color(1.0f, 0.0f, 0.0f, 1.0f));
        button.onClick.AddListener(UpgradeModule);
    }

    public void SetLevelText()
    {
        _levelText.text = "LV. " + level + " / " + MaxLevelList[_type-1];
    }

    public void SetPriceText()
    {
        if (level >= MaxLevelList[_type-1])
        {
            button.interactable = false;
            _priceText.text = "MAX";
        }
        else
        {
            price = (level + 1) * PriceList[_type-1];
            _priceText.text = price.ToString();
            if (price > PlayerPrefs.GetInt("TotalCoin", 0))
            {
                _priceText.color = Color.red;
                button.interactable = false;
            }
            else
            {
                _priceText.color = Color.black;
                button.interactable = true;
            }
        }
    }

    public void SetAnnaeText(string s, Color c)
    {
        _annaeText.color = c;
        _annaeText.text = s;
    }

    public void SetDescriptionText()
    {
        if (_type == 1)
        {
            _descriptionText.text = $"{ModuleTextList[_type-1]}+{0.02 * level}";
        }
        else if (_type == 2 || _type == 3 || _type == 6)
        {
            _descriptionText.text = $"{ModuleTextList[_type-1]}+{0.5 * level}%";
        }
        else if (_type == 4 || _type == 5)
        {
            _descriptionText.text = $"{ModuleTextList[_type-1]}+{level}";
        }
        else if (_type == 7)
        {
            _descriptionText.text = $"{ModuleTextList[_type-1]}-{0.05 * level}";
        }
        else if (_type == 8)
        {
            _descriptionText.text = $"{ModuleTextList[_type-1]}-{0.5 * level})%";
        }
    }

    public void UpgradeModule()
    {
        int coins = PlayerPrefs.GetInt("TotalCoin", 0);
        if (coins >= price && level < MaxLevelList[_type-1])
        {
            coins -= price;
            level += 1;
            PlayerPrefs.SetInt($"module_{_type}", level);
            PlayerPrefs.SetInt("TotalCoin", coins);
            SetLevelText();
            SetPriceText();
            SetDescriptionText();
            SetAnnaeText("업그레이드 완료", Color.green);

            foreach (Module module in FindObjectsOfType<Module>())
            {
                module.SetPriceText();
            }
            CoinText.text = $"Coins : {coins}";
            MusicPlayer.PlayOneShot(_sfxUpgrade);
        }
        else
        {
            SetAnnaeText("코인이 부족합니다.", Color.red);
            MusicPlayer.PlayOneShot(_sfxFail);
        }
    }
}
