using UnityEngine;
using TMPro;
using Audio;
using Core.Constants;
using Gameplay.Collectibles;

namespace Gameplay.Managers
{
    public class ExpManager : MonoBehaviour
    {
        SoundManager Sound => GameManager.Instance.SoundManager;
        
        public int Coins;
        public float ExpCurrent = 0;
        public int ExpMax = 10;
        [SerializeField] TMP_Text resourceText;
        [SerializeField] GameObject choicePrefab;
        [HideInInspector] public bool Hextech = false;

        private void Start()
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
                Sound.PlaySFX(SoundKey.Exp);
            }
            SetText();
        }

        public void LevelUp()
        {
            GameStateManager.Instance.SetState(GameState.Paused);
            ExpCurrent -= ExpMax;
            ExpMax += 5;
            Instantiate(choicePrefab, Vector3.zero, Quaternion.identity);
            Sound.PlaySFX(SoundKey.Levelup);
            Coins += 5;
            SetText();
        }
    }
}
