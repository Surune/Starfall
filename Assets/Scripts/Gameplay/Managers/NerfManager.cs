using System;
using UnityEngine;
using Core.Constants;
using Gameplay.Entities;
using Gameplay.Spawning;

namespace Gameplay.Managers
{
    public class NerfManager : MonoBehaviour
    {
        Player player => GameManager.Instance.Player;
        Spawner spawner => GameManager.Instance.Spawner;
        Timer timer => GameManager.Instance.Timer;
        PlayerManager playerManager => GameManager.Instance.PlayerManager;
        HPManager hp => GameManager.Instance.HPManager;

        public int NerfLevel;
        public int HighestLevel;

        private void Start()
        {
            NerfLevel = PlayerPrefs.GetInt("currentLevel", 0);
            HighestLevel = PlayerPrefs.GetInt("highestLevel", 0);
            SetSupernova();
        }

        private void SetSupernova()
        {
            for (var i = 1; i <= NerfLevel; i++)
            {
                switch (i)
                {
                    case 20:
                        hp.Lethal = true;
                        break;
                    case 19:
                        Enemy.DamageCoefficient -= 0.05f;
                        break;
                    case 18:
                        Enemy.ItemProb = 1f;
                        break;
                    case 17:
                        player.ChangeSkillCool(player.SkillCooltimeMax + 0.05f);
                        break;
                    case 16:
                        throw new NotImplementedException("spawnsmall");
                        //spawner.SpawnSmall = true;
                        break;
                    case 15:
                        spawner.SpawnRandom = true;
                        break;
                    case 14:
                        playerManager.damage -= 0.05f;
                        break;
                    case 13:
                        playerManager.criticalCoefficient -= 0.05f;
                        playerManager.criticalProb -= 0.05f;
                        break;
                    case 12:
                        spawner.AddHP += 2;
                        break;
                    case 9:
                        playerManager.damage -= 0.05f;
                        break;
                    case 8:
                        player.ChangeSkillCool(player.SkillCooltimeMax * 1.05f);
                        break;
                    case 7:
                        playerManager.damageCoefficient = 0.95f;
                        break;
                    case 6:
                        playerManager.criticalProb -= 0.1f;
                        break;
                    case 5:
                        playerManager.criticalCoefficient -= 0.1f;
                        break;
                    case 4:
                        spawner.SpeedCoefficient += 0.1f;
                        break;
                    case 3:
                        timer.Addition -= 1;
                        break;
                    case 1:
                        hp.MaxHP = 80f;
                        hp.CurrentHP = 80f;
                        break;
                }
            }
        }

        public void Cleared()
        {
            if (NerfLevel == HighestLevel && HighestLevel < ConstantStore.NERF_TEXT_LIST.Length)
            {
                PlayerPrefs.SetInt("highestLevel", HighestLevel + 1);
            }
        }
    }
}
