using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour {
    public TextMeshProUGUI coinText;
    private int currentPilot = 0;
    public Transform[] characters;

    private AudioSource musicPlayer;
    [SerializeField] private AudioClip sfxPurchase;
    [SerializeField] private AudioClip sfxSelect;
    
    private void Start()
    {
        musicPlayer = GameObject.Find("SoundManager").GetComponent<AudioSource>();
        currentPilot = PlayerPrefs.GetInt("currentPilot");
        for (int i = 0; i < characters.Length; i++){
            bool having = System.Convert.ToBoolean(PlayerPrefs.GetInt("pilot_" + (i+1)));
            if (having) {
                characters[i].GetChild(3).gameObject.SetActive(false);
                characters[i].GetChild(4).gameObject.SetActive(true);
            }
            else {
                characters[i].GetChild(3).gameObject.SetActive(true);
                characters[i].GetChild(4).gameObject.SetActive(false);
            }
        }
        SelectCharacter(currentPilot);
    }

    public void PurchaseCharacter(int num) {
        int coins = PlayerPrefs.GetInt("TotalCoin", 0);
        if (coins >= 1500) {
            coins -= 1500;
            PlayerPrefs.SetInt("TotalCoin", coins);
            PlayerPrefs.SetInt("pilot_" + num, System.Convert.ToInt16(true));
            PlayerPrefs.Save();
            characters[num-1].GetChild(4).gameObject.SetActive(true);
            characters[num-1].GetChild(3).gameObject.SetActive(false);
            coinText.text = "Coins : " + coins;
            musicPlayer.PlayOneShot(sfxPurchase);
        }
    }

    public void SelectCharacter(int num) {
        currentPilot = num;
        PlayerPrefs.SetInt("currentPilot", num);
        PlayerPrefs.Save();
        for (int i = 0; i < characters.Length; i++){
            if (i == num-1) {
                characters[i].GetChild(4).gameObject.GetComponent<Button>().interactable = false;
            }
            else {
                characters[i].GetChild(4).gameObject.GetComponent<Button>().interactable = true;
            }
        }
        musicPlayer.PlayOneShot(sfxSelect);
    }
}
