using System;
using System.Collections.Generic;
using UnityEngine;
using Starfall.Entity;
using Starfall.Constants;
using Random = UnityEngine.Random;

namespace Starfall.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        Player player => GameManager.Instance.Player;
        HPManager hp => GameManager.Instance.HPManager;
        SFXManager SfxManager => GameManager.Instance.SfxManager;
        ExpManager exp => GameManager.Instance.ExpManager;
        AbilityManager ability => GameManager.Instance.AbilityManager;

        static GameObject EnemyList => GameManager.Instance.Spawner.EnemyList;
        public int refresh = 0;
        public float fixDamage = 0f;
        public float damage = 1f;
        public float damageCoefficient = 1f;
        public float criticalProb = 0f;
        public float criticalCoefficient = 1.5f;
        public float fatalProb = 0f;
        public float shotSpeedCoefficient = 1f;
        public int criticalCount = 0;
        public bool statikk = false;
        public bool aquaris = false;
        public bool repair = false;
        public bool jera = false;
        public bool dagaz = false;
        public bool reinforce = false;
        public List<Wing> Wings;

        [SerializeField] GameObject _wingPrefab;
        [SerializeField] Transform _wingTransform;
        int shotnum = 0;

        void Start()
        {
            SetPlayer();
        }

        void SetPlayer()
        {
            var currentPlayer = PlayerPrefs.GetInt("currentPilot", 1);
            switch (currentPlayer)
            {
                case 1:
                    exp.Coins = -5;
                    exp.ExpCurrent = exp.ExpMax;
                    exp.LevelUp();
                    break;
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
            if (currentPlayer != 1)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }
            exp.SetText();
        }

        public void DamageAllEnemy(float dmg)
        {
            SfxManager.PlayEnemySound(isCritical : false, isKilled : false);
            foreach (Transform t in GameManager.GetAllChilds(EnemyList.transform))
            {
                if (t.gameObject.CompareTag("Enemy"))
                {
                    t.gameObject.GetComponent<Enemy>().GetDamage(dmg, critical : false, mute : true);
                }
            }
        }

        public void MakeCritical(Bullet bullet)
        {
            bullet.IsCritical = true;
            bullet.Damage *= criticalCoefficient;
            bullet.Burst = ability.burst;
            if (ability.nuker && criticalProb >= 1f)
            {
                bullet.Damage *= criticalProb;
            }
            if (ability.assassination)
            {
                bullet.Penetrate = true;
            }
        }

        public void SetFireInfo(Bullet bullet)
        {
            shotnum++;
            if (statikk && shotnum % 50 == 0)
            {
                DamageAllEnemy(damage * damageCoefficient + fixDamage);
            }
            if (aquaris)
            {
                bullet.IsCritical = true;
            }

            var rand = Random.value;
            bullet.IsFatal = rand <= fatalProb;
            bullet.Damage = damage;

            if (rand <= criticalProb || (ability.luckySeven && shotnum % 7 == 0))
            {
                MakeCritical(bullet);
            }
            else
            {
                bullet.IsCritical = false;
            }

            if (ability.third && shotnum % 3 == 0)
            {
                bullet.Damage += 0.3f;
            }
            bullet.Damage *= damageCoefficient;
            if (ability.penetrate)
            {
                bullet.Penetrate = true;
            }
            bullet.Psychosink = ability.psychosink;
            bullet.Beingstronger = ability.beingstronger;
            bullet.Udo = ability.udo;
            bullet.Freezing = ability.freezing;
            bullet.Damage += fixDamage;
        }

        public void GetWing(int num)
        {
            for (int i = 0; i < num; i++)
            {
                var w = Instantiate(_wingPrefab, _wingTransform);
                Wings.Add(w.GetComponent<Wing>());
            }
        }
    }
}
