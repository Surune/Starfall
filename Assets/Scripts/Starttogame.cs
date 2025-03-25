using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class Starttogame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] TextMeshProUGUI versionText;

    void Start()
    {
        PlayerPrefs.SetInt("pilot_1", 1);
        highscoreText.text = $"High Score : {PlayerPrefs.GetInt("HighScore", 0)}"
                            + $"\nSupernova : {PlayerPrefs.GetInt("highestLevel", 0)}"
                            + "\n\nCopyright 2022. Surune\nAll rights reserved.";
        versionText.text = $"Ver {Application.version}";
    }
}
