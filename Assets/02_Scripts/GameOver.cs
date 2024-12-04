using UnityEngine;
using TMPro;
using Starfall.Manager;

public class GameOver : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioClip sfxEmerge;
    
    void Start() {
        GameManager.Instance.soundManager.gameObject.GetComponent<AudioSource>().PlayOneShot(sfxEmerge);
        string text = "Now Score : " + PlayerPrefs.GetInt("NowScore");
        text += "\n\nHigh Score : " + PlayerPrefs.GetInt("HighScore");
        text += "\n\nCoins : " + PlayerPrefs.GetInt("TotalCoin") + " (+" + PlayerPrefs.GetInt("Coin") + ")";
        scoreText.text = text;
    }
}