using UnityEngine;
using TMPro;
using Starfall.Utils;
using Starfall.Entity;
using Starfall.Constants;

namespace Starfall.Manager {
    public class NerfManager {
        #region Managers
        private Player _player;
        private Spawner _spawner;
        private Timer _timer;
        private PlayerManager _playerManager;
        private HPManager _hp;
        private ExpManager _exp;
        #endregion

        #region Properties
        public int nerfLevel;
        public int highestLevel;
        public GameObject nerftext;
        public Transform content;
        public bool hardmode;
        #endregion

        public NerfManager(
            Player player,
            Spawner spawner,
            Timer timer,
            PlayerManager playerManager,
            HPManager hp,
            ExpManager exp)
        {
            _player = player;
            _spawner = spawner;
            _timer = timer;
            _playerManager = playerManager;
            _hp = hp;
            _exp = exp;

            nerfLevel = PlayerPrefs.GetInt("currentLevel", 0);
            highestLevel = PlayerPrefs.GetInt("highestLevel", 0);
            hardmode = System.Convert.ToBoolean(PlayerPrefs.GetInt("hardMode", 0));
            
            Invoke("SetSupernova", 0f);
            if(hardmode) {
                GameManager.Instance.coinCoefficient += 0.5f;
                ChoiceButton.hardmode = true;
                StartCoroutine(SetHardmode());
            }
            else {
                ChoiceButton.hardmode = false;
            }
        }

        private System.Collections.IEnumerator SetHardmode() {
            GameObject choice = GameObject.Find("Choice(Clone)");
            if (choice != null) {
                yield return new WaitUntil(() => choice == null);
            }
            
            if (hardmode) {
                int r = Random.Range(0, ConstantStore.HARDMODE_TEXT_LIST.Length);
                GameObject obj = Instantiate(nerftext, content);
                switch(r) {
                    case 0:
                        _spawner.meteorCoefficient *= 1.25f;
                        break;
                    case 1:
                        _spawner.speedCoefficient += 0.2f;
                        break;
                    case 2:
                        _spawner.addHP += 2f;
                        break;
                    case 3:
                        _timer.addition -= 1;
                        break;
                }
                obj.transform.GetComponent<TextMeshProUGUI>().text = $"- 별의 시련 -\n\n{ConstantStore.HARDMODE_TEXT_LIST[r]}";
                obj.GetComponent<Tween>().tween();
            }
        }

        private void SetSupernova() {
            for (int i = 1; i <= nerfLevel; i++) {
                switch (i) {
                    case 20:
                        _hp.lethal = true;
                        break;
                    case 19:
                        _spawner.damageCoefficient -= 0.05f;
                        break;
                    case 18:
                        Enemy.itemProb = 1f;
                        break;
                    case 17:
                        _player.ChangeSkillCool(_player.skillCooltimeMax + 0.05f);
                        break;
                    case 16:
                        _spawner.spawnSmall = true;
                        break;
                    case 15:
                        _spawner.spawnRandom = true;
                        break;
                    case 14:
                        _playerManager.damage -= 0.05f;
                        break;
                    case 13:
                        _playerManager.criticalCoefficient -= 0.05f;
                        _playerManager.criticalProb -= 0.05f;
                        break;
                    case 12:
                        _spawner.addHP += 2;
                        break;
                    case 11:
                        _spawner.meteorCoefficient *= 1.25f;
                        break;
                    case 10:
                        _spawner.makeMeteor = true;
                        break;
                    case 9:
                        _playerManager.fixDamage -= 0.05f;
                        break;
                    case 8:
                        _player.ChangeSkillCool(_player.skillCooltimeMax * 1.05f);
                        break;
                    case 7:
                        _playerManager.damageCoefficient = 0.95f;
                        break;
                    case 6:
                        _playerManager.criticalProb -= 0.1f;
                        break;
                    case 5:
                        _playerManager.criticalCoefficient -= 0.1f;
                        break;
                    case 4:
                        _spawner.speedCoefficient += 0.1f;
                        break;
                    case 3:
                        _timer.addition -= 1;
                        break;
                    case 2:
                        _exp.expMax += 5;
                        break;
                    case 1:
                        _hp.maxHP = 80f;
                        _hp.currentHP = 80f;
                        break;
                    default:
                        break;
                }
            }
            _exp.SetText();
        }

        public void Cleared() {
            if(nerfLevel == highestLevel && highestLevel < ConstantStore.NERF_TEXT_LIST.Length){
                PlayerPrefs.SetInt("highestLevel", highestLevel + 1);
            }
        }
    }
}