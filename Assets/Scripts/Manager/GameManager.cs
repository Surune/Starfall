using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Starfall.Entity;

namespace Starfall.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameStateManager GameStateManager;
        public EffectManager EffectManager;
        public SFXManager SfxManager;
        public PoolManager PoolManager;
        public PlayerManager PlayerManager;
        public AbilityManager AbilityManager;
        public NerfManager NerfManager;
        public Player Player;
        public Timer Timer;
        public ExpManager ExpManager;
        public ScoreManager ScoreManager;
        public BackendManager BackendManager;
        public Spawner Spawner;
        public HPManager HPManager;

        public Transform EnemyList = null!;
        public Transform FireballList = null!;
        [HideInInspector] public int ActiveChoiceNum = 0;
        [HideInInspector] public int ActiveEnemyNum = 0;
        [HideInInspector] public List<int> AbilityNumbers = new();
        public float CoinCoefficient = 1f;

        void Awake()
        {
            Instance = this;
            BackendManager = new();
            GameStateManager = new();

            // 업그레이드 적용
            // 모듈 1 : 공격력 +0.02
            PlayerManager.damage += 0.02f * PlayerPrefs.GetInt("module_1");
            // 모듈 2 : 치명타 확률 +0.5%
            PlayerManager.criticalProb = 0.005f * PlayerPrefs.GetInt("module_2");
            // 모듈 3 : 치명타 대미지 +0.5%
            PlayerManager.criticalCoefficient += 0.005f * PlayerPrefs.GetInt("module_3");
            // 모듈 4 : 새로고침 횟수 추가
            PlayerManager.refresh += PlayerPrefs.GetInt("module_4");
            // 모듈 6 : 코인 획득량 + 1%
            CoinCoefficient += 0.01f * PlayerPrefs.GetInt("module_6");
            // 모듈 7 : 적 체력 -0.05
            Spawner.AddHP -= PlayerPrefs.GetInt("module_7") * 0.05f;
            // 모듈 8 : 적 속도 -0.5%
            Spawner.SpeedCoefficient -= PlayerPrefs.GetInt("module_8", 0) * 0.005f;
        }

        public static Transform FindClosestTransform(List<Transform> t_list, Vector3 pos)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            foreach (Transform t in t_list)
            {
                float dist = Vector3.Distance(t.position, pos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }
            return tMin;
        }

        public static List<Transform> GetAllChilds(Transform _t)
        {
            return _t.Cast<Transform>().Where(t => t.gameObject.activeSelf == true).ToList();
        }

        public void GameOver(int coin)
        {
            GetComponent<AudioSource>().Pause();
            GetCoin(0);
            BackendManager.UploadGameData(false);
        }

        public void GameClear(int coin)
        {
            GetComponent<AudioSource>().Pause();
            NerfManager.Cleared();
            GetCoin(coin);
            BackendManager.UploadGameData(true);
        }

        void GetCoin(int bonus)
        {
            var nowscore = (int)ScoreManager.TotalScore;

            if (bonus != 0)
            {
                CoinCoefficient += 0.05f * NerfManager.NerfLevel;
                nowscore = Mathf.CeilToInt(nowscore * (1 + 0.05f * NerfManager.NerfLevel));
            }
            var coins = Mathf.CeilToInt((ExpManager.Coins + bonus) * CoinCoefficient);
            if (nowscore > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", nowscore);
            }
            PlayerPrefs.SetInt("NowScore", nowscore);
            PlayerPrefs.SetInt("Coin", coins);
            var totalcoin = PlayerPrefs.GetInt("TotalCoin") + coins;
            PlayerPrefs.SetInt("TotalCoin", totalcoin);
            PlayerPrefs.Save();
        }
    }
}
