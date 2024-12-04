using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Starfall.Utils;
using Starfall.Entity;

namespace Starfall.Manager {
    public class AbilityManager {
        #region Managers
        private readonly Player _player;
        private readonly HPManager _hp;
        private static Spawner spawner => GameManager.Instance.spawner;
        private static ExpManager expmanager => GameManager.Instance.exp;
        private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
        private readonly Timer _timer;
        #endregion

        [SerializeField] private GameObject abilityPrefab;
        [SerializeField] private string abilityCSVFile;
        public List<Dictionary<string, object>> abilityList;
        public Sprite[] abilitySprites;
        public List<GameObject> abilities;
        private int abilityCount;

        [HideInInspector] public int[] synergy;
        public Sprite[] SynergySprites;
        public Color[] SynergyColors;
        
        [SerializeField] private GameObject content;

        public bool assassination = false;
        public bool penetrate = false;
        public bool luckySeven = false;
        public bool beingstronger = false;
        public bool third = false;
        public bool nuker = false;
        public bool culling = false;
        public bool noxious = false;
        public bool psychosense = false;
        public bool psychosink = false;
        public bool freezing = false;
        public bool locked = false;
        public bool awaken = false;
        public bool burst = false;
        public bool echo = false;
        public bool immolation = false;
        public bool explode = false;
        public bool magnet = false;
        public bool burning = false;
        public bool kineticBarrage = false;
        public bool udo = false;
        public bool fifth = false;
        public bool fracture = false;
        public bool firm = false;

        public int[] synergyRequirement = {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4};
        [SerializeField] private AudioSource _musicplayer;
        [SerializeField] private AudioClip sfxSynergy;

        public AbilityManager(Player player, HPManager hp, Timer timer) {
            _player = player;
            _hp = hp;
            _timer = timer;

            abilityList = CSVReader.Read(abilityCSVFile);
            abilityCount = -1;
            abilities = new List<GameObject>();
            abilitySprites = Resources.LoadAll<Sprite>("IconImages");
            synergy = new int[11];
        }

        private void GetSynergy(int synergy_type){
            synergy[synergy_type]++;
            if (synergy_type != 0 && synergy[synergy_type] == synergyRequirement[synergy_type-1]) {
                _musicplayer.PlayOneShot(sfxSynergy);
                switch(synergy_type){
                    case 1:
                        spawner.damageCoefficient += 0.1f;
                        break;
                    case 2:
                        spawner.disabled = true;
                        break;
                    case 3:
                        playerManager.statikk = true;
                        break;
                    case 4:
                        _hp
            .maxHP += 30;
                        _hp
            .currentHP = _hp
            .maxHP;
                        _hp
            .SetHealthBar();
                        break;
                    case 5:
                        _hp
            .GetBarrier(5);
                        break;
                    case 6:
                        spawner.speedCoefficient -= 0.1f;
                        break;
                    case 7:
                        playerManager.fatalProb += 0.2f;
                        break;
                    case 8:
                        playerManager.fixDamage += 1f;
                        break;
                    case 9:
                        playerManager.criticalProb = playerManager.criticalProb < 1f ? 1f : playerManager.criticalProb;
                        break;
                    case 10:
                        playerManager.GetWing(2);
                        break;
                }
            }
        }

        public void Choiced(int id) {
            GameManager.Instance.AbilityNumbers.Add(id);
            abilities.Add(Instantiate(abilityPrefab, content.transform));
            abilityCount++;
            Ability now_ability = abilities[abilityCount].GetComponent<Ability>();
            GetChildWithName(abilities[abilityCount], "Icon").GetComponent<Image>().sprite = abilitySprites[id];
            now_ability.abilityID = id;
            now_ability.abilityType = int.Parse(abilityList[id]["ABILITY TYPE"].ToString());
            GetSynergy(int.Parse(abilityList[id]["SYNERGY1"].ToString()));
            GetSynergy(int.Parse(abilityList[id]["SYNERGY2"].ToString()));
            now_ability.abilityImage.color = SynergyColors[int.Parse(abilityList[id]["SYNERGY1"].ToString())];
            switch (id) {
                case 0: //amplify
                    playerManager.criticalProb += 0.3f;
                    spawner.meteorCoefficient += 0.2f;
                    break;
                case 1: //amplify_x
                    playerManager.criticalCoefficient += 0.3f;
                    spawner.meteorCoefficient += 0.2f;
                    break;
                case 2: //annihilation
                    now_ability.skillCooltimeMax = 0.5f;
                    break;
                case 3: //arrow
                    now_ability.skillCooltimeMax = 1f;
                    break;
                case 4: //assassination
                    playerManager.criticalProb += 0.05f;
                    assassination = true;
                    break;
                case 5: //awakening
                    awaken = true;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 1.7f);
                    break;
                case 6: //balista
                    playerManager.criticalProb += 0.2f;
                    break;
                case 7: //balista_x
                    playerManager.criticalCoefficient += 0.2f;
                    break;
                case 8: //berserking
                    _hp
        .berserk = true;
                    break;
                case 9: //blessing
                    playerManager.damage += 0.5f;
                    playerManager.criticalProb += 0.1f;
                    break;
                case 10: //blunt_arrow
                    _hp
        .GetBarrier(1);
                    _hp
        .blunt = true;
                    break;
                case 11: //burning_field
                    burning = true;
                    break;
                case 12: //burst
                    burst = true;
                    break;
                case 13: //call_of_the_void
                    playerManager.damage += 0.5f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax - 0.01f);
                    break;
                case 14: //carving_shield
                    _hp
        .GetBarrier(1);
                    _hp
        .carving = true;
                    break;
                case 15: //celestial_shot
                    playerManager.fixDamage += playerManager.damage * 0.5f;
                    playerManager.damage *= 0.5f;
                    break;
                case 16: //centipede
                    playerManager.damageCoefficient *= 0.5f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.5f);
                    break;
                case 17: //chronomancy
                    now_ability.skillCooltimeMax = 4f;
                    break;
                case 18: //cluster
                    playerManager.damageCoefficient += 0.05f;
                    playerManager.criticalProb += 0.05f;
                    playerManager.criticalCoefficient += 0.05f;
                    break;
                case 19: //conjurer
                    now_ability.skillCooltimeMax = 4f;
                    break;
                case 20: //construct
                    playerManager.criticalProb += 0.005f * (_player.killNum > 60 ? 60 : _player.killNum);
                    break;
                case 21: //crucio
                    now_ability.skillCooltimeMax = 5f;
                    break;
                case 22: //culling_shot
                    culling = true;
                    break;
                case 23: //curser
                    playerManager.fatalProb += 0.1f;
                    break;
                case 24: //damage_4
                    now_ability.skillCooltimeMax = 2.5f;
                    break;
                case 25: //death
                    _hp
        .ChangeHP(-66);
                    playerManager.damageCoefficient += 0.5f;
                    break;
                case 26: //deceleration
                    spawner.speedCoefficient *= 0.8f;
                    break;
                case 27: //defence
                    _hp
        .GetBarrier(3);
                    _hp
        .currentHP = _hp
        .maxHP;
                    _hp
        .SetHealthBar();
                    break;
                case 28: //dividends
                    playerManager.shotSpeedCoefficient *= 0.5f;
                    playerManager.damageCoefficient += 0.1f;
                    break;
                case 29: //echo_barrage
                    echo = true;
                    break;
                case 30: //enchanted
                    playerManager.fixDamage += 0.2f;
                    playerManager.criticalProb += 0.1f;
                    break;
                case 31: //enchanter
                    playerManager.damage += 0.8f;
                    break;
                case 32: //explode
                    explode = true;
                    break;
                case 33: //explorer
                    Enemy.itemProb += 6f;
                    break;
                case 34: //firm
                    playerManager.fatalProb += 0.05f;
                    firm = true;
                    break;
                case 35: //flying_dagger
                    penetrate = true;
                    break;
                case 36: //forcer
                    playerManager.damage += 0.2f;
                    playerManager.fixDamage += 0.1f;
                    break;
                case 37: //forcer_x
                    playerManager.damage += 0.5f;
                    playerManager.damageCoefficient += 0.05f;
                    break;
                case 38: //fracture
                    fracture = true;
                    break;
                case 39: //freezing
                    freezing = true;
                    break;
                case 40: //good_luck
                    playerManager.refresh += 3;
                    GameManager.Instance.coinCoefficient += 0.25f;
                    break;
                case 41: //gravity
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.9f);
                    playerManager.criticalProb += 0.1f;
                    break;
                case 42: //greed
                    Enemy.itemProb += 3f;
                    GameManager.Instance.coinCoefficient += 0.2f;
                    break;
                case 43: //hardening
                    now_ability.skillCooltimeMax = 15f;
                    break;
                case 44: //haste
                    beingstronger = true;
                    break;
                case 45: //healing
                    now_ability.skillCooltimeMax = 15f;
                    break;
                case 46: //heavy_impact
                    playerManager.shotSpeedCoefficient *= 0.5f;
                    playerManager.criticalProb += 0.3f;
                    break;
                case 47: //hexagon
                    now_ability.skillCooltimeMax = 6f;
                    break;
                case 48: //hextech
                    expmanager.hextech = true;
                    break;
                case 49: //hive
                    playerManager.GetWing(2);
                    break;
                case 50: //immolation
                    playerManager.damageCoefficient += 0.005f * (_player.killNum > 60 ? 60 : _player.killNum);
                    break;
                case 51: //intimidation
                    playerManager.damageCoefficient += 0.5f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 1.5f);
                    break;
                case 52: //journey
                    Enemy.itemProb += 0.3f;
                    _hp
        .GetBarrier(1);
                    playerManager.refresh += 1;
                    _timer.addition += 1;
                    break;
                case 53: //kinetic_barrage
                    kineticBarrage = true;
                    playerManager.fatalProb += 0.05f;
                    break;
                case 54: //lightning_strike
                    playerManager.shotSpeedCoefficient *= 0.8f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.8f);
                    break;
                case 55: //lock
                    now_ability.skillCooltimeMax = 4f;
                    break;
                case 56: //longshot
                    playerManager.criticalProb += 0.1f;
                    playerManager.criticalCoefficient += 0.1f;
                    break;
                case 57: //lucky_seven
                    luckySeven = true;
                    break;
                case 58: //machine_arrow
                    playerManager.criticalProb -= 0.1f;
                    playerManager.fatalProb += 0.15f;
                    break;
                case 59: //mage
                    playerManager.shotSpeedCoefficient *= 0.5f;
                    playerManager.damage += 1f;
                    break;
                case 60: //magnetism
                    magnet = true;
                    spawner.addHP -= 1;
                    break;
                case 61: //magnitude
                    spawner.addHP -= 0.5f;
                    _timer.addition += 1;
                    break;
                case 62: //meat_shield
                    _hp
        .GetBarrier(1);
                    _hp
        .meatshield = true;
                    break;
                case 63: //mercenary
                    now_ability.skillCooltimeMax = 25f;
                    break;
                case 64: //noxious_shot
                    noxious = true;
                    break;
                case 65: //nuker
                    nuker = true;
                    break;
                case 66: //orbitism
                    now_ability.skillCooltimeMax = 10f;
                    break;
                case 67: //ouroboros_l
                    _hp
        .GetBarrier(1);
                    _hp
        .our_l = true;
                    break;
                case 68: //ouroboros_r
                    _hp
        .GetBarrier(1);
                    _hp
        .our_r = true;
                    break;
                case 69: //payback
                    playerManager.fatalProb += 0.15f;
                    playerManager.damageCoefficient -= 0.1f;
                    break;
                case 70: //pointing
                    now_ability.skillCooltimeMax = 10f;
                    break;
                case 71: //porcupine
                    _hp
        .porcupine = true;
                    break;
                case 72: //psycholeak
                    playerManager.GetWing(1);
                    Enemy.itemProb += 3f;
                    break;
                case 73: //psychosense
                    psychosense = true;
                    break;
                case 74: //psychoshot
                    playerManager.criticalCoefficient += 0.1f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.95f);
                    break;
                case 75: //psychosink
                    playerManager.damageCoefficient = playerManager.damageCoefficient > 0.8f ? (playerManager.damageCoefficient - 0.8f) : 0.01f;
                    psychosink = true;
                    break;
                case 76: //psyker
                    udo = true;
                    playerManager.shotSpeedCoefficient *= 1.25f;
                    break;
                case 77: //psychic_orbs
                    now_ability.skillCooltimeMax = 1f;
                    break;
                case 78: //punishment
                    playerManager.fatalProb += 0.15f;
                    playerManager.fixDamage -= 0.05f;
                    break;
                case 79: //ranger
                    now_ability.skillCooltimeMax = 1f;
                    break;
                case 80: //rearm
                    now_ability.skillCooltimeMax = 10f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.75f);
                    break;
                case 81: //reinforce
                    playerManager.reinforce = true;
                    break;
                case 82: //resonance
                    Choiced((int)Random.Range(0, abilitySprites.Length));
                    Choiced((int)Random.Range(0, abilitySprites.Length));
                    Choiced((int)Random.Range(0, abilitySprites.Length));
                    return;
                case 83: //rogue
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.85f);
                    break;
                case 84: //seeping
                    now_ability.skillCooltimeMax = 10f;
                    break;
                case 85: //shoot_5
                    fifth = true;
                    break;
                case 86: //sorcerer
                    playerManager.shotSpeedCoefficient *= 2f;
                    spawner.meteorCoefficient /= 2f;
                    break;
                case 87: //speed_3
                    third = true;
                    break;
                case 88: //speed_booster
                    playerManager.criticalProb += 0.1f;
                    playerManager.shotSpeedCoefficient *= 1.25f;
                    break;
                case 89: //stability
                    playerManager.criticalCoefficient += 0.01f * (_player.killNum > 60 ? 60 : _player.killNum);
                    break;
                case 90: //stunning_strike
                    playerManager.fatalProb += 0.15f;
                    spawner.meteorCoefficient += 0.25f;
                    break;
                case 91: //swarm
                    now_ability.skillCooltimeMax = 5f;
                    break;
                case 92: //taunt
                    playerManager.fatalProb += 0.5f;
                    _hp
        .lethal = true;
                    break;
                case 93: //tremor
                    spawner.meteorCoefficient += 0.25f;
                    expmanager.expMax -= 10;
                    break;
                case 94: //ultimatum
                    spawner.meteorCoefficient += 0.25f;
                    playerManager.damageCoefficient += 0.25f;
                    break;
                case 95: //unleash
                    spawner.speedCoefficient *= 1.1f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.8f);
                    break;
                case 96: //unrelenting
                    now_ability.skillCooltimeMax = 4f;
                    break;
                case 97: //unwavering
                    playerManager.damage += 0.5f;
                    playerManager.criticalCoefficient += 0.1f;
                    break;
                case 98: //void
                    now_ability.skillCooltimeMax = 5f;
                    break;
                case 99: //vulnerability
                    _hp
        .maxHP = Mathf.CeilToInt(_hp
        .maxHP * 0.65f);
                    _hp
        .currentHP = Mathf.CeilToInt(_hp
        .currentHP * 0.65f);
                    _hp
        .SetHealthBar();
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.65f);
                    break;
                case 100: //warrior
                    playerManager.damageCoefficient += 0.2f;
                    _hp
        .SetHealthBar();
                    break;
                case 101: //volcano
                    spawner.makeMeteor = true;
                    playerManager.damageCoefficient += 0.3f;
                    break;
                case 102: //berkano
                    playerManager.GetWing(1);
                    playerManager.damage += 0.1f * GameObject.FindGameObjectsWithTag("Wing").Length;
                    break;
                case 103: //ehwaz
                    playerManager.damage += 1f;
                    _timer.roundNum += 1;
                    break;
                case 104: //ansuz
                    playerManager.fixDamage += 0.5f;
                    break;
                case 105: //hagalaz
                    spawner.disabled = true;
                    break;
                case 106: //jera
                    playerManager.jera = true;
                    break;
                case 107: //dagaz
                    playerManager.dagaz = true;
                    break;
                case 108: //algiz
                    _hp
        .GetBarrier(5);
                    break;
                case 109: //perthro
                    playerManager.refresh += 10;
                    break;
                case 110: //shard
                    playerManager.damage += 0.1f;
                    playerManager.criticalProb += 0.03f;
                    playerManager.criticalCoefficient += 0.03f;
                    _hp
        .GetBarrier(1);
                    break;
                case 111: //spirit
                    playerManager.damage += 0.01f * (_player.killNum > 60 ? 60 : _player.killNum);
                    break;
                case 112: //cancer
                    _player.ChangeSkillCool(_player.skillCooltimeMax - 0.05f);
                    break;
                case 113: //leo
                    _hp
        .maxHP *= 2f;
                    _hp
        .currentHP = _hp
        .maxHP;
                    _hp
        .SetHealthBar();
                    break;
                case 114: //taurus
                    now_ability.skillCooltimeMax = 10f;
                    break;
                case 115: //aries
                    playerManager.GetWing(1);
                    _hp
        .GetBarrier(1);
                    break;
                case 116: //virgo
                    _hp
        .virgo = true;
                    break;
                case 117: //libra
                    var temp = Enemy.itemProb;
                    Enemy.itemProb = playerManager.criticalProb;
                    playerManager.criticalProb = temp;
                    break;
                case 118: //sagittarius
                    spawner.addHP -= 2f;
                    break;
                case 119: //capricon
                    _hp
        .GetBarrier(1);
                    _hp
        .capricon = true;
                    break;
                case 120: //aquaris
                    playerManager.criticalProb += 0.05f;
                    playerManager.aquaris = true;
                    break;
                case 121: //pisces
                    playerManager.fixDamage += 0.1f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.9f);
                    break;
                case 122: //scorpio
                    playerManager.fatalProb += 0.15f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax + 0.05f);
                    break;
                case 123: //gemini
                    spawner.spawnSmall = true;
                    _timer.addition += 3;
                    break;
                case 124: //ophiuchus
                    spawner.spawnRandom = true;
                    playerManager.damage += 1f;
                    break;
                case 125: //pluto
                    spawner.meteorCoefficient += 0.25f;
                    _player.ChangeSkillCool(_player.skillCooltimeMax * 0.75f);
                    break;
                case 126: //overload
                    playerManager.fixDamage *= 2f;
                    break;
                case 127: //horn
                    playerManager.GetWing(1);
                    WingBullet.damage += 0.5f;
                    break;
                case 128: //blank
                    playerManager.fatalProb += 0.05f;
                    Fireball.fatalDamage += 0.5f;
                    break;
                case 129: //contract
                    playerManager.GetWing(1);
                    WingBullet.speed *= 1.5f;
                    playerManager.shotSpeedCoefficient *= 1.5f;
                    break;
                case 130: //repair
                    playerManager.repair = true;
                    break;
                case 131: //heat
                    playerManager.GetWing(1);
                    Wing[] wings = GameObject.Find("Wings").transform.GetComponentsInChildren<Wing>();
                    foreach (Wing w in wings) {
                        w.ChangeSkillCool(Wing.skillCooltimeMax * 0.8f);
                    }
                    break;
                case 132: //lower
                    playerManager.GetWing(1);
                    Wing.freezing = true;
                    break;
                case 133: //overclock
                    playerManager.GetWing(1);
                    WingBullet.udo = true;
                    break;
                case 134: //tick
                    Enemy.damageCoefficient += 0.1f;
                    break;
                case 135: //supply
                    now_ability.skillCooltimeMax = 10f;
                    break;
            }
        }

        GameObject GetChildWithName(GameObject obj, string name) {
            Transform childTrans = obj.transform.Find(name);
            if (childTrans != null) return childTrans.gameObject;
            else                    return null;
        }
    }
}