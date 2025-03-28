using UnityEngine;
using TMPro;
using Starfall.Constants;

namespace Starfall.Manager
{
    public class ExpManager : MonoBehaviour
    {
        static SFXManager sfx => GameManager.Instance.SfxManager;
        public int Coins;
        public float ExpCurrent = 0;
        public int ExpMax = 10;
        [SerializeField] TextMeshProUGUI resourceText;
        [SerializeField] AudioSource musicPlayer;
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
                sfx.PlayExp();
            }
            SetText();
        }

        public void LevelUp()
        {
            GameStateManager.Instance.SetState(GameState.Paused);
            ExpCurrent -= ExpMax;
            ExpMax += 5;
            Instantiate(choicePrefab, Vector3.zero, Quaternion.identity);
            sfx.PlayBonus();
            Coins += 5;
            SetText();
        }
    }
}
