using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using Audio;
using Core.Constants;
using Gameplay.Entities;
using Gameplay.Spawning;
using Utilities;

namespace Gameplay.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        Player player => GameManager.Instance.Player;
        HPManager hp => GameManager.Instance.HPManager;
        SoundManager Sound => GameManager.Instance.SoundManager;
        Spawner Spawner => GameManager.Instance.Spawner;
        
        public int refresh = 0;
        public float damage = 1f;
        public float damageCoefficient = 1f;
        public float criticalProb = 0f;
        public float criticalCoefficient = 1.5f;
        public float fatalProb = 0f;
        public float shotSpeedCoefficient = 1f;
        public bool statikk = false;
        public bool aquaris = false;
        public bool repair = false;
        public bool jera = false;
        public bool dagaz = false;
        public bool reinforce = false;
        public List<Wing> Wings;

        [SerializeField] Wing wingPrefab;
        [SerializeField] Transform wingContent;
        int shotnum = 0;

        private void Start()
        {
            SetPlayer();
        }

        private void SetPlayer()
        {
            var currentPlayer = PlayerPrefs.GetInt("currentPilot", 1);
            switch (currentPlayer)
            {
                case 2:
                    criticalProb += 0.2f;
                    break;
                case 3:
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.8f);
                    break;
                case 4:
                    hp.GetBarrier(5);
                    break;
                case 5:
                    damage += 1f;
                    break;
                case 6:
                    GameManager.Instance.CoinCoefficient += 0.5f;
                    break;
            }

            GameStateManager.Instance.SetState(GameState.Gameplay);
        }

        public void DamageAllEnemy(float dmg)
        {
            Sound.PlaySFX(SoundKey.EnemyHit);
            foreach (var t in Spawner.ActiveEnemies)
            {
                t.GetDamage(dmg, critical : false, mute : true);
            }
        }

        private void MakeCritical(Bullet bullet)
        {
            bullet.IsCritical = true;
            bullet.Damage *= criticalCoefficient;
        }

        public void SetFireInfo(Bullet bullet)
        {
            shotnum++;
            if (statikk && shotnum % 50 == 0)
            {
                DamageAllEnemy(damage * damageCoefficient);
            }
            if (aquaris)
            {
                bullet.IsCritical = true;
            }

            var rand = Random.value;
            bullet.IsFatal = rand <= fatalProb;
            bullet.Damage = damage;

            if (rand <= criticalProb)
            {
                MakeCritical(bullet);
            }
            else
            {
                bullet.IsCritical = false;
            }

            bullet.Damage *= damageCoefficient;
        }

        public void GetWing(int num)
        {
            for (int i = 0; i < num; i++)
            {
                var wing = Instantiate(wingPrefab, wingContent);
                GameManager.Instance.InjectDependency(wing);
                Wings.Add(wing);
            }
        }
    }
}
