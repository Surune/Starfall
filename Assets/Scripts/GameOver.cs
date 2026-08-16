using UnityEngine;
using TMPro;
using Starfall.Manager;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] AudioClip sfxEmerge;

    void Start()
    {
        scoreText.text = $"\n\nCoins : {PlayerPrefs.GetInt("TotalCoin")} (+{PlayerPrefs.GetInt("Coin")})";
    }
}
