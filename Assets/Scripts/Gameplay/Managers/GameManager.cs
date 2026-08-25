using System;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using Core.Constants;
using Data.Abilities;
using Gameplay.Effects;
using Gameplay.Entities;
using Gameplay.Spawning;
using UI;

namespace Gameplay.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameStateManager GameStateManager = new ();
        public EffectManager EffectManager;
        public SoundManager SoundManager;
        public PoolManager PoolManager;
        public PlayerManager PlayerManager;
        public AbilityManager AbilityManager;
        public NerfManager NerfManager;
        public Player Player;
        public Timer Timer;
        public Spawner Spawner;
        public HPManager HPManager;

        [SerializeField] private SoundDictionary soundDictionary;
        [SerializeField] private GameObject gameClearDisplay;
        [HideInInspector] public int ActiveEnemyNum = 0;
        [HideInInspector] public int Coins = 0;
        [HideInInspector] public List<AbilityData> SelectedAbilities = new();
        public float CoinCoefficient = 1f;
        private void Awake()
        {
            Instance = this;
            SoundManager = new(soundDictionary);
            PoolManager.SetObjectInitializer(ConfigurePooledObject);
            Spawner.Initialize(PoolManager, Timer, RegisterEnemySpawned);
            EffectManager.Initialize(PoolManager);
        }

        public void SetLocalPlayer(Player player)
        {
            Player = player;
            PlayerManager = player.GetComponent<PlayerManager>();
            ConfigurePlayer(player);
            AbilityManager.Initialize(player, PlayerManager, HPManager, Spawner, this);
        }

        private void ConfigurePlayer(Player player)
        {
            player.Initialize(PlayerManager, PoolManager, SoundManager, GameStateManager);

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
            // 모듈 8 : 적 속도 -0.5%
            Spawner.SpeedCoefficient -= PlayerPrefs.GetInt("module_8", 0) * 0.005f;
        }

        public void ConfigureWing(Wing wing)
        {
            wing.Initialize(PoolManager, GameStateManager);
        }

        private void ConfigurePooledObject(Component pooledComponent)
        {
            switch (pooledComponent)
            {
                case Enemy enemy:
                    var deathResolver = new EnemyDeathResolver(EffectManager, PoolManager, Player, Spawner, Timer, RegisterEnemyRemoved);
                    enemy.Initialize(EffectManager, GameStateManager, HPManager, SoundManager, Timer, deathResolver);
                    break;
                case Bullet bullet:
                    bullet.Initialize(PlayerManager, GameStateManager, Spawner);
                    break;
                case WingBullet wingBullet:
                    wingBullet.Initialize(GameStateManager, Spawner);
                    break;
                case DropItem dropItem:
                    dropItem.Initialize(HPManager, PlayerManager, SoundManager, GameStateManager);
                    break;
                case DamageEffect damageEffect:
                    damageEffect.Initialize(GameStateManager);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pooledComponent));
            }
        }

        public void RegisterEnemySpawned()
        {
            ActiveEnemyNum++;
        }

        public void RegisterEnemyRemoved()
        {
            ActiveEnemyNum--;
        }

        public void GameOver(int coin)
        {
            GetComponent<AudioSource>().Pause();
            GetCoin(0);
        }

        public void GameClear(int coin)
        {
            GetComponent<AudioSource>().Pause();
            NerfManager.Cleared();
            GetCoin(coin);
            GameStateManager.SetState(GameState.Paused);
            Instantiate(gameClearDisplay, Vector3.zero, Quaternion.identity);
        }

        private void GetCoin(int bonus)
        {
            if (bonus != 0)
            {
                CoinCoefficient += 0.05f * NerfManager.NerfLevel;
            }
            
            var coins = Mathf.CeilToInt((Coins + bonus) * CoinCoefficient);
            PlayerPrefs.SetInt("Coin", coins);
            
            var totalcoin = PlayerPrefs.GetInt("TotalCoin") + coins;
            PlayerPrefs.SetInt("TotalCoin", totalcoin);
            PlayerPrefs.Save();
        }
    }
}
