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
        EffectManager effect => GameManager.Instance.EffectManager;
        ExpManager exp => GameManager.Instance.ExpManager;
        AbilityManager ability => GameManager.Instance.AbilityManager;
        Spawner spawner => GameManager.Instance.Spawner;

        static GameObject EnemyList => GameManager.Instance.Spawner.EnemyList;
        public SpriteRenderer spr;
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
                    spr.color = Color.white;
                    exp.Coins = -5;
                    exp.ExpCurrent = exp.ExpMax;
                    exp.LevelUp();
                    break;
                case 2:
                    spr.color = spawner.ColorList[0];
                    criticalProb += 0.2f;
                    break;
                case 3:
                    spr.color = spawner.ColorList[2];
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.8f);
                    break;
                case 4:
                    spr.color = spawner.ColorList[3];
                    hp.GetBarrier(5);
                    break;
                case 5:
                    spr.color = spawner.ColorList[4];
                    damage += 1f;
                    break;
                case 6:
                    spr.color = spawner.ColorList[6];
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
            effect.PlayEnemySound(isCritical : false, isKilled : false);
            foreach (Transform t in GameManager.GetAllChilds(EnemyList.transform))
            {
                if (t.gameObject.CompareTag("Enemy"))
                {
                    t.gameObject.GetComponent<Enemy>().GetDamage(dmg, critical : false, mute : true);
                }
            }
        }

        public void MakeCritical(Fireball fireball)
        {
            fireball.IsCritical = true;
            fireball.Damage *= criticalCoefficient;
            fireball.Burst = ability.burst;
            if (ability.nuker && criticalProb >= 1f)
            {
                fireball.Damage *= criticalProb;
            }
            if (ability.assassination)
            {
                fireball.Penetrate = true;
            }
        }

        public void SetFireInfo(Fireball fireball)
        {
            shotnum++;
            if (statikk && shotnum % 50 == 0)
            {
                DamageAllEnemy(damage * damageCoefficient + fixDamage);
            }
            if (aquaris)
            {
                fireball.IsCritical = true;
            }
            float rand = Random.value;
            if (rand <= fatalProb)
            {
                fireball.IsFatal = true;
            }
            else
            {
                fireball.IsFatal = false;
            }

            fireball.Damage = damage;
            if (rand <= criticalProb || (ability.luckySeven && shotnum % 7 == 0))
            {
                MakeCritical(fireball);
            }
            else
            {
                fireball.IsCritical = false;
            }

            if (ability.third && shotnum % 3 == 0)
            {
                fireball.Damage += 0.3f;
            }
            fireball.Damage *= damageCoefficient;
            if (ability.penetrate)
            {
                fireball.Penetrate = true;
            }
            fireball.Psychosink = ability.psychosink;
            fireball.Beingstronger = ability.beingstronger;
            fireball.Udo = ability.udo;
            fireball.Freezing = ability.freezing;
            fireball.Damage += fixDamage;
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
