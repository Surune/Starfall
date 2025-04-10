using UnityEngine;
using TMPro;
using Starfall.Utils;
using Starfall.Entity;
using Starfall.Constants;
using System.Collections;

namespace Starfall.Manager
{
    public class NerfManager : MonoBehaviour
    {
        Player player => GameManager.Instance.Player;
        Spawner spawner => GameManager.Instance.Spawner;
        Timer timer => GameManager.Instance.Timer;
        PlayerManager playerManager => GameManager.Instance.PlayerManager;
        HPManager hp => GameManager.Instance.HPManager;
        ExpManager exp => GameManager.Instance.ExpManager;

        public int NerfLevel;
        public int HighestLevel;
        public GameObject Nerftext;
        public Transform Content;

        void Start()
        {
            NerfLevel = PlayerPrefs.GetInt("currentLevel", 0);
            HighestLevel = PlayerPrefs.GetInt("highestLevel", 0);
            SetSupernova();
        }

        void SetSupernova()
        {
            for (int i = 1; i <= NerfLevel; i++)
            {
                switch (i)
                {
                    case 20:
                        hp.Lethal = true;
                        break;
                    case 19:
                        spawner.DamageCoefficient -= 0.05f;
                        break;
                    case 18:
                        Enemy.ItemProb = 1f;
                        break;
                    case 17:
                        player.ChangeSkillCool(player.SkillCooltimeMax + 0.05f);
                        break;
                    case 16:
                        spawner.SpawnSmall = true;
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
                    case 11:
                        spawner.MeteorCoefficient *= 1.25f;
                        break;
                    case 10:
                        spawner.MakeMeteor = true;
                        break;
                    case 9:
                        playerManager.fixDamage -= 0.05f;
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
                    case 2:
                        exp.ExpMax += 5;
                        break;
                    case 1:
                        hp.MaxHP = 80f;
                        hp.CurrentHP = 80f;
                        break;
                    default:
                        break;
                }
            }
            exp.SetText();
        }

        public void Cleared()
        {
            if(NerfLevel == HighestLevel && HighestLevel < ConstantStore.NERF_TEXT_LIST.Length)
            {
                PlayerPrefs.SetInt("highestLevel", HighestLevel + 1);
            }
        }
    }
}
