using UnityEngine;
using TMPro;
using Starfall.Constants;

namespace Starfall.Manager
{
    public class ExpManager : MonoBehaviour
    {
        public int Coins;
        public float ExpCurrent = 0;
        public int ExpMax = 10;
        [SerializeField] TextMeshProUGUI resourceText;
        [SerializeField] AudioSource musicPlayer;
        [SerializeField] AudioClip sfxBonus;
        [SerializeField] AudioClip sfxExp;
        [SerializeField] GameObject choicePrefab;
        [HideInInspector] public bool Hextech = false;

        void Start()
        {
            Coins = 0;
            ExpCurrent = 0;
            ExpMax = 10;
            ExpCurrent += PlayerPrefs.GetInt("module_5");
            SetText();
        }

        public void SetText()
        {
            resourceText.text = $"{(int)ExpCurrent}/{ExpMax}";
        }

        public void GetExp(int num)
        {
            ExpCurrent += num;

            if (ExpCurrent >= ExpMax)
            {
                if (Hextech)
                {
                    GameManager.Instance.PlayerManager.refresh += 1;
                }
                LevelUp();
            }
            else
            {
                musicPlayer.PlayOneShot(sfxExp);
            }
            SetText();
        }

        public void LevelUp()
        {
            GameStateManager.Instance.SetState(GameState.Paused);
            ExpCurrent -= ExpMax;
            ExpMax += 5;
            Instantiate(choicePrefab, Vector3.zero, Quaternion.identity);
            musicPlayer.PlayOneShot(sfxBonus);
            Coins += 5;
            SetText();
        }
    }
}
