using System.Collections.Generic;
using UnityEngine;
using Starfall.Entity;

namespace Starfall.Manager {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        public GameStateManager gameStateManager => GameStateManager.Instance;
        public SoundManager soundManager;
        public EffectManager EffectManager;
        public PoolManager PoolManager;
        public PlayerManager PlayerManager;
        public AbilityManager AbilityManager;
        public NerfManager NerfManager;
        public Player Player;
        public Timer timer;
        public ExpManager exp;
        public ScoreManager ScoreManager;
        public BackendManager Backend;
        public Spawner spawner;
        public HPManager HPManager;
        
        public Transform enemyList = null!;
        public Transform fireballList = null!;
        [HideInInspector] public int activeChoiceNum = 0;
        [HideInInspector] public int activeEnemyNum = 0;
        [HideInInspector] public List<int> AbilityNumbers = new List<int>();
        public float coinCoefficient = 1f;

        void Awake() {
            Instance = this;
            PoolManager = new();
            soundManager = new();
            EffectManager = new(PoolManager);
            PlayerManager = new(Player, HPManager, EffectManager, exp, AbilityManager);
            AbilityManager = new(PlayerManager, HPManager, timer);
            NerfManager = new(Player, spawner, timer, PlayerManager, HPManager, exp);

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
            coinCoefficient += 0.01f * PlayerPrefs.GetInt("module_6");
            // 모듈 7 : 적 체력 -0.05
            spawner.addHP -= PlayerPrefs.GetInt("module_7") * 0.05f;
            // 모듈 8 : 적 속도 -0.5%
            spawner.speedCoefficient -= PlayerPrefs.GetInt("module_8", 0) * 0.005f ;
            soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            soundManager.ApplyMute();
        }

        public static Transform FindClosestTransform(List<Transform> t_list, Vector3 pos) {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            foreach (Transform t in t_list) {
                float dist = Vector3.Distance(t.position, pos);
                if (dist < minDist) {
                    tMin = t;
                    minDist = dist;
                }
            }
            return tMin;
        }

        public static List<Transform> GetAllChilds(Transform _t) {
            List<Transform> ts = new List<Transform>();
            foreach (Transform t in _t) {
                if(t.gameObject.activeSelf == true) {
                    ts.Add(t);
                }
            }
            return ts;
        }

        public void GameOver(int coin) {
            GetComponent<AudioSource>().Pause();
            GetCoin(0);
            Backend.UploadGameData(false);
        }

        public void GameClear(int coin) {
            GetComponent<AudioSource>().Pause();
            NerfManager.Cleared();
            GetCoin(coin);
            Backend.UploadGameData(true);
        }

        public void GetCoin(int bonus) {
            int nowscore = (int)ScoreManager.totalScore;
            
            if (bonus != 0) {
                coinCoefficient += 0.05f * NerfManager.nerfLevel;
                nowscore = Mathf.CeilToInt(nowscore * (1 + 0.05f * NerfManager.nerfLevel));
            }
            int coins = Mathf.CeilToInt((exp.coins + bonus) * coinCoefficient);
            if (nowscore > PlayerPrefs.GetInt("HighScore")) PlayerPrefs.SetInt("HighScore", nowscore);
            PlayerPrefs.SetInt("NowScore", nowscore);
            PlayerPrefs.SetInt("Coin", coins);
            int totalcoin = PlayerPrefs.GetInt("TotalCoin") + coins;
            PlayerPrefs.SetInt("TotalCoin", totalcoin);
            PlayerPrefs.Save();
        }
    }
}