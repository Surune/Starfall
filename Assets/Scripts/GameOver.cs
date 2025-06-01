using UnityEngine;
using TMPro;
using Starfall.Manager;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] AudioClip sfxEmerge;

    void Start()
    {
        string text = $"Now Score : {PlayerPrefs.GetInt("NowScore")}";
        text += $"\n\nHigh Score : {PlayerPrefs.GetInt("HighScore")}";
        text += $"\n\nCoins : {PlayerPrefs.GetInt("TotalCoin")} (+{PlayerPrefs.GetInt("Coin")})";
        scoreText.text = text;
    }
}
