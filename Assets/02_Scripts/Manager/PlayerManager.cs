using System.Collections.Generic;
using UnityEngine;
using Starfall.Entity;

namespace Starfall.Manager {
    public class PlayerManager {
        #region Managers
        private readonly Player _player;
        private readonly HPManager _hp;
        private readonly EffectManager _effect;
        private readonly ExpManager _exp;
        private readonly AbilityManager _ability;
        #endregion

        public SpriteRenderer spr;
        public int refresh = 0;
        public float fixDamage = 0f;
        public float damage = 1f;
        public float damageCoefficient = 1f;
        public float criticalProb = 0f;
        public float criticalCoefficient = 1.5f;
        public float fatalProb = 0f;
        public float shotSpeedCoefficient = 1f;
        private int shotnum = 0;
        public int criticalCount = 0;
        private Transform EnemyList;
        public GameObject wingPrefab;
        public Transform wingTransform;
        public List<Wing> wings;
        private Spawner spawner;
        private Timer timer;
        public bool statikk = false;
        public bool aquaris = false;
        public bool repair = false;
        public bool jera = false;
        public bool dagaz = false;
        public bool reinforce = false;

        public PlayerManager(
            Player player, 
            HPManager hp, 
            EffectManager effect, 
            ExpManager exp,
            AbilityManager ability)
        {
            _player = player;
            _hp = hp;
            _effect = effect;
            _exp = exp;
            _ability = ability;

            EnemyList = GameObject.Find("Enemies").transform;
            timer = GameObject.Find("Tsimer").GetComponent<Timer>();
            spawner = GameObject.Find("Spawner").GetComponent<Spawner>();

            SetPlayer();
        }

        private void SetPlayer() {
            int currentPlayer = PlayerPrefs.GetInt("currentPilot", 1);
            switch (currentPlayer) {
                case 1:
                    spr.color = Color.white;
                    _exp.coins = -5;
                    _exp.expCurrent = _exp.expMax;
                    _exp.LevelUp();
                    break;
                case 2:
                    spr.color = spawner.colorList[0];
                    criticalProb += 0.2f;
                    break;
                case 3:
                    spr.color = spawner.colorList[2];
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.8f);
                    break;
                case 4:
                    spr.color = spawner.colorList[3];
                    _hp.GetBarrier(5);
                    break;
                case 5:
                    spr.color = spawner.colorList[4];
                    damage += 1f;
                    break;
                case 6:
                    spr.color = spawner.colorList[6];
                    GameManager.Instance.coinCoefficient += 0.5f;
                    break;
            }
            if (currentPlayer != 1)
                GameStateManager.Instance.SetState(GameState.Gameplay);
            _exp.SetText();
        }
        
        public void DamageAllEnemy(float dmg) {
            _effect
.PlayEnemySound(isCritical : false, isKilled : false);
            foreach (Transform t in GameManager.GetAllChilds(EnemyList)) {
                if (t.gameObject.tag == "Enemy") {
                    t.gameObject.GetComponent<Enemy>().GetDamage(dmg, critical : false, mute : true);
                }
            }
        }

        public void MakeCritical(Fireball fireball) {
            fireball.isCritical = true;
            fireball.damage *= criticalCoefficient;
            fireball.burst = _ability.burst;
            if (_ability.nuker && criticalProb >= 1f) fireball.damage *= criticalProb;
            if (_ability.assassination)    fireball.penetrate = true;
        }

        public void SetFireInfo(Fireball fireball) {
            shotnum++;
            if (statikk && shotnum % 50 == 0) 
                DamageAllEnemy(damage * damageCoefficient + fixDamage);
            if (aquaris) fireball.isCritical = true;
            float rand = Random.value;
            if (rand <= fatalProb) fireball.isFatal = true;
            else fireball.isFatal = false;
            
            fireball.damage = damage;
            if (rand <= criticalProb || (_ability.luckySeven && shotnum % 7 == 0))
                MakeCritical(fireball);
            else
                fireball.isCritical = false;
            
            if (_ability.third && shotnum % 3 == 0) fireball.damage += 0.3f;
            fireball.damage *= damageCoefficient;
            if (_ability.penetrate) {
                fireball.penetrate = true;
            }
            fireball.psychosink = _ability.psychosink;
            fireball.beingstronger = _ability.beingstronger;
            fireball.udo = _ability.udo;
            fireball.freezing = _ability.freezing;
            fireball.damage += fixDamage;
        }

        public void GetWing(int num) {
            for(int i = 0; i < num; i++) {
                var w = Instantiate(wingPrefab, wingTransform);
                wings.Add(w.GetComponent<Wing>());
            }
        }
    }
}