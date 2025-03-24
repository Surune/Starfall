using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;

public class CharacterManager : MonoBehaviour
{
    AudioSource MusicPlayer => GameManager.Instance.Player.MusicPlayer;
    [SerializeField] Transform[] _characters;
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] AudioClip _sfxPurchase;
    [SerializeField] AudioClip _sfxSelect;
    int currentPilot = 0;

    void Start()
    {
        currentPilot = PlayerPrefs.GetInt("currentPilot");
        for (var i = 0; i < _characters.Length; i++)
        {
            var having = System.Convert.ToBoolean(PlayerPrefs.GetInt($"pilot_{i + 1}"));
            if (having)
            {
                _characters[i].GetChild(3).gameObject.SetActive(false);
                _characters[i].GetChild(4).gameObject.SetActive(true);
            }
            else
            {
                _characters[i].GetChild(3).gameObject.SetActive(true);
                _characters[i].GetChild(4).gameObject.SetActive(false);
            }
        }
        SelectCharacter(currentPilot);
    }

    public void PurchaseCharacter(int num)
    {
        var coins = PlayerPrefs.GetInt("TotalCoin", 0);
        if (coins < 1500)
        {
            return;
        }

        coins -= 1500;
        PlayerPrefs.SetInt("TotalCoin", coins);
        PlayerPrefs.SetInt($"pilot_{num}", System.Convert.ToInt16(true));
        PlayerPrefs.Save();
        _characters[num-1].GetChild(4).gameObject.SetActive(true);
        _characters[num-1].GetChild(3).gameObject.SetActive(false);
        _coinText.text = $"Coins : {coins}";
        MusicPlayer.PlayOneShot(_sfxPurchase);
    }

    public void SelectCharacter(int num)
    {
        currentPilot = num;
        PlayerPrefs.SetInt("currentPilot", num);
        PlayerPrefs.Save();
        for (var i = 0; i < _characters.Length; i++)
        {
            _characters[i].GetChild(4).gameObject.GetComponent<Button>().interactable = (i != num - 1);
        }
        MusicPlayer.PlayOneShot(_sfxSelect);
    }
}
