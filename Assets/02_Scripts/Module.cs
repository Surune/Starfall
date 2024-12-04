using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Module : MonoBehaviour {
    [SerializeField] private int type;
    [SerializeField] private TextMeshProUGUI PriceText;
    private int price;
    [SerializeField] private TextMeshProUGUI LevelText;
    private int level;
    public Button button;
    [SerializeField] private TextMeshProUGUI AnnaeText;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    private TextMeshProUGUI CoinText;
    private AudioSource musicPlayer;
    [SerializeField] private AudioClip sfx_upgrade;
    [SerializeField] private AudioClip sfx_fail;

    private static string[] ModuleTextList = {"공격력\n", "치명타 확률\n", "치명타 대미지\n", "새로고침\n", "시작 경험치\n", "코인 획득량\n", "적 체력\n", "적 속도\n"};
    private static int[] MaxLevelList = {50, 50, 50, 5, 10, 50, 20, 20};
    private static int[] PriceList = {10, 10, 10, 100, 30, 20, 25, 25};

    void Start() {
        musicPlayer = GameObject.Find("SoundManager").GetComponent<AudioSource>();
        level = PlayerPrefs.GetInt("module_" + type);
        CoinText = GameObject.Find("Coin").GetComponent<TextMeshProUGUI>();
        SetLevelText();
        SetPriceText();
        SetDescriptionText();
        SetAnnaeText("", new Color(1.0f, 0.0f, 0.0f, 1.0f));
        button.onClick.AddListener(UpgradeModule);
    }

    public void SetLevelText() {
        LevelText.text = "LV. " + level + " / " + MaxLevelList[type-1];
    }

    public void SetPriceText() {
        if (level >= MaxLevelList[type-1]) {
            button.interactable = false;
            PriceText.text = "MAX";
        }
        else {
            price = (level + 1) * PriceList[type-1];
            PriceText.text = price.ToString();
            if (price > PlayerPrefs.GetInt("TotalCoin", 0)) {
                PriceText.color = Color.red;
                button.interactable = false;
            }
            else {
                PriceText.color = Color.black;
                button.interactable = true;
            }
        }
    }

    public void SetAnnaeText(string s, Color c) {
        AnnaeText.color = c;
        AnnaeText.text = s;
    }

    public void SetDescriptionText() {
        if (type == 1) {
            DescriptionText.text = ModuleTextList[type-1] + "+" + (0.02*level);
        }
        else if (type == 2 || type == 3 || type == 6) {
            DescriptionText.text = ModuleTextList[type-1] + "+" + (0.5*level) + "%";
        }
        else if (type == 4 || type == 5) {
            DescriptionText.text = ModuleTextList[type-1] + level;
        }
        else if (type == 7){
            DescriptionText.text = ModuleTextList[type-1] + "-" + (0.05*level);
        }
        else if (type == 8){
            DescriptionText.text = ModuleTextList[type-1] + "-" + (0.5*level) + "%";
        }
    }

    public void UpgradeModule() {
        int coins = PlayerPrefs.GetInt("TotalCoin", 0);
        if (coins >= price && level < MaxLevelList[type-1]) {
            coins -= price;
            level += 1;
            PlayerPrefs.SetInt("module_" + type, level);
            PlayerPrefs.SetInt("TotalCoin", coins);
            SetLevelText();
            SetPriceText();
            SetDescriptionText();
            SetAnnaeText("업그레이드 완료", Color.green);
            
            foreach (Module module in FindObjectsOfType<Module>())
            {
                module.SetPriceText();
            }
            CoinText.text = "Coins : " + coins;
            musicPlayer.PlayOneShot(sfx_upgrade);
        }
        else {
            SetAnnaeText("코인이 부족합니다.", Color.red);
            musicPlayer.PlayOneShot(sfx_fail);
        }
    }
}
