using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Starfall.Utils;
using Starfall.Entity;
using Starfall.Constants;
using Random = UnityEngine.Random;

namespace Starfall.Manager
{
    public class AbilityManager : MonoBehaviour
    {
        Player player => GameManager.Instance.Player;
        HPManager hp => GameManager.Instance.HPManager;
        Spawner spawner => GameManager.Instance.Spawner;
        ExpManager exp => GameManager.Instance.ExpManager;
        PlayerManager playerManager => GameManager.Instance.PlayerManager;
        Timer timer => GameManager.Instance.Timer;

        public List<Dictionary<string, object>> AbilityList;
        public Sprite[] AbilitySprites;
        public List<GameObject> Abilities;
        int abilityCount;

        [HideInInspector] public int[] Synergy;
        public Sprite[] SynergySprites;
        public Color[] SynergyColors;

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

        public int[] SynergyRequirement = {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4};

        [SerializeField] GameObject abilityPrefab;
        [SerializeField] string abilityCSVFileName;
        [SerializeField] GameObject content;
        [SerializeField] AudioSource musicPlayer;
        [SerializeField] AudioClip sfxSynergy;

        void Start()
        {
            AbilityList = CSVReader.Read(abilityCSVFileName);
            abilityCount = -1;
            Abilities = new List<GameObject>();
            AbilitySprites = Resources.LoadAll<Sprite>("IconImages");
            Synergy = new int[11];
        }

        void GetSynergy(int synergyType)
        {
            Synergy[synergyType]++;
            if (synergyType == 0 || Synergy[synergyType] != SynergyRequirement[synergyType - 1])
            {
                return;
            }

            musicPlayer.PlayOneShot(sfxSynergy);
            switch (synergyType)
            {
                case 1:
                    spawner.DamageCoefficient += 0.1f;
                    break;
                case 2:
                    spawner.Disabled = true;
                    break;
                case 3:
                    playerManager.statikk = true;
                    break;
                case 4:
                    hp.MaxHP += 30;
                    hp.CurrentHP = hp.MaxHP;
                    hp.SetHealthBar();
                    break;
                case 5:
                    hp.GetBarrier(5);
                    break;
                case 6:
                    spawner.SpeedCoefficient -= 0.1f;
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

        public void Choiced(int id)
        {
            GameManager.Instance.AbilityNumbers.Add(id);
            Abilities.Add(Instantiate(abilityPrefab, content.transform));
            abilityCount++;
            var currentAbility = Abilities[abilityCount].GetComponent<Ability>();
            GetChildWithName(Abilities[abilityCount], "Icon").GetComponent<Image>().sprite = AbilitySprites[id];
            currentAbility.AbilityID = id;
            currentAbility.AbilityType = (AbilityType)int.Parse(AbilityList[id]["ABILITY TYPE"].ToString());
            GetSynergy(int.Parse(AbilityList[id]["SYNERGY1"].ToString()));
            GetSynergy(int.Parse(AbilityList[id]["SYNERGY2"].ToString()));
            currentAbility.AbilityImage.color = SynergyColors[int.Parse(AbilityList[id]["SYNERGY1"].ToString())];

            switch (id)
            {
                case 0: //amplify
                    playerManager.criticalProb += 0.3f;
                    spawner.MeteorCoefficient += 0.2f;
                    break;
                case 1: //amplify_x
                    playerManager.criticalCoefficient += 0.3f;
                    spawner.MeteorCoefficient += 0.2f;
                    break;
                case 2: //annihilation
                    currentAbility.SkillCooltimeMax = 0.5f;
                    break;
                case 3: //arrow
                    currentAbility.SkillCooltimeMax = 1f;
                    break;
                case 4: //assassination
                    playerManager.criticalProb += 0.05f;
                    assassination = true;
                    break;
                case 5: //awakening
                    awaken = true;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 1.7f);
                    break;
                case 6: //balista
                    playerManager.criticalProb += 0.2f;
                    break;
                case 7: //balista_x
                    playerManager.criticalCoefficient += 0.2f;
                    break;
                case 8: //berserking
                    hp.Berserk = true;
                    break;
                case 9: //blessing
                    playerManager.damage += 0.5f;
                    playerManager.criticalProb += 0.1f;
                    break;
                case 10: //blunt_arrow
                    hp.GetBarrier(1);
                    hp.Blunt = true;
                    break;
                case 11: //burning_field
                    burning = true;
                    break;
                case 12: //burst
                    burst = true;
                    break;
                case 13: //call_of_the_void
                    playerManager.damage += 0.5f;
                    player.ChangeSkillCool(player.SkillCooltimeMax - 0.01f);
                    break;
                case 14: //carving_shield
                    hp.GetBarrier(1);
                    hp.Carving = true;
                    break;
                case 15: //celestial_shot
                    playerManager.fixDamage += playerManager.damage * 0.5f;
                    playerManager.damage *= 0.5f;
                    break;
                case 16: //centipede
                    playerManager.damageCoefficient *= 0.5f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.5f);
                    break;
                case 17: //chronomancy
                    currentAbility.SkillCooltimeMax = 4f;
                    break;
                case 18: //cluster
                    playerManager.damageCoefficient += 0.05f;
                    playerManager.criticalProb += 0.05f;
                    playerManager.criticalCoefficient += 0.05f;
                    break;
                case 19: //conjurer
                    currentAbility.SkillCooltimeMax = 4f;
                    break;
                case 20: //construct
                    playerManager.criticalProb += 0.005f * (player.KillNum > 60 ? 60 : player.KillNum);
                    break;
                case 21: //crucio
                    currentAbility.SkillCooltimeMax = 5f;
                    break;
                case 22: //culling_shot
                    culling = true;
                    break;
                case 23: //curser
                    playerManager.fatalProb += 0.1f;
                    break;
                case 24: //damage_4
                    currentAbility.SkillCooltimeMax = 2.5f;
                    break;
                case 25: //death
                    hp.ChangeHP(-66);
                    playerManager.damageCoefficient += 0.5f;
                    break;
                case 26: //deceleration
                    spawner.SpeedCoefficient *= 0.8f;
                    break;
                case 27: //defence
                    hp.GetBarrier(3);
                    hp.CurrentHP = hp.MaxHP;
                    hp.SetHealthBar();
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
                    Enemy.ItemProb += 6f;
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
                    GameManager.Instance.CoinCoefficient += 0.25f;
                    break;
                case 41: //gravity
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.9f);
                    playerManager.criticalProb += 0.1f;
                    break;
                case 42: //greed
                    Enemy.ItemProb += 3f;
                    GameManager.Instance.CoinCoefficient += 0.2f;
                    break;
                case 43: //hardening
                    currentAbility.SkillCooltimeMax = 15f;
                    break;
                case 44: //haste
                    beingstronger = true;
                    break;
                case 45: //healing
                    currentAbility.SkillCooltimeMax = 15f;
                    break;
                case 46: //heavy_impact
                    playerManager.shotSpeedCoefficient *= 0.5f;
                    playerManager.criticalProb += 0.3f;
                    break;
                case 47: //hexagon
                    currentAbility.SkillCooltimeMax = 6f;
                    break;
                case 48: //hextech
                    exp.Hextech = true;
                    break;
                case 49: //hive
                    playerManager.GetWing(2);
                    break;
                case 50: //immolation
                    playerManager.damageCoefficient += 0.005f * (player.KillNum > 60 ? 60 : player.KillNum);
                    break;
                case 51: //intimidation
                    playerManager.damageCoefficient += 0.5f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 1.5f);
                    break;
                case 52: //journey
                    Enemy.ItemProb += 0.3f;
                    hp.GetBarrier(1);
                    playerManager.refresh += 1;
                    timer.Addition += 1;
                    break;
                case 53: //kinetic_barrage
                    kineticBarrage = true;
                    playerManager.fatalProb += 0.05f;
                    break;
                case 54: //lightning_strike
                    playerManager.shotSpeedCoefficient *= 0.8f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.8f);
                    break;
                case 55: //lock
                    currentAbility.SkillCooltimeMax = 4f;
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
                    spawner.AddHP -= 1;
                    break;
                case 61: //magnitude
                    spawner.AddHP -= 0.5f;
                    timer.Addition += 1;
                    break;
                case 62: //meat_shield
                    hp.GetBarrier(1);
                    hp.Meatshield = true;
                    break;
                case 63: //mercenary
                    currentAbility.SkillCooltimeMax = 25f;
                    break;
                case 64: //noxious_shot
                    noxious = true;
                    break;
                case 65: //nuker
                    nuker = true;
                    break;
                case 66: //orbitism
                    currentAbility.SkillCooltimeMax = 10f;
                    break;
                case 67: //ouroboros_l
                    hp.GetBarrier(1);
                    hp.Our_l = true;
                    break;
                case 68: //ouroboros_r
                    hp.GetBarrier(1);
                    hp.Our_r = true;
                    break;
                case 69: //payback
                    playerManager.fatalProb += 0.15f;
                    playerManager.damageCoefficient -= 0.1f;
                    break;
                case 70: //pointing
                    currentAbility.SkillCooltimeMax = 10f;
                    break;
                case 71: //porcupine
                    hp.Porcupine = true;
                    break;
                case 72: //psycholeak
                    playerManager.GetWing(1);
                    Enemy.ItemProb += 3f;
                    break;
                case 73: //psychosense
                    psychosense = true;
                    break;
                case 74: //psychoshot
                    playerManager.criticalCoefficient += 0.1f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.95f);
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
                    currentAbility.SkillCooltimeMax = 1f;
                    break;
                case 78: //punishment
                    playerManager.fatalProb += 0.15f;
                    playerManager.fixDamage -= 0.05f;
                    break;
                case 79: //ranger
                    currentAbility.SkillCooltimeMax = 1f;
                    break;
                case 80: //rearm
                    currentAbility.SkillCooltimeMax = 10f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.75f);
                    break;
                case 81: //reinforce
                    playerManager.reinforce = true;
                    break;
                case 82: //resonance
                    Choiced((int)Random.Range(0, AbilitySprites.Length));
                    Choiced((int)Random.Range(0, AbilitySprites.Length));
                    Choiced((int)Random.Range(0, AbilitySprites.Length));
                    return;
                case 83: //rogue
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.85f);
                    break;
                case 84: //seeping
                    currentAbility.SkillCooltimeMax = 10f;
                    break;
                case 85: //shoot_5
                    fifth = true;
                    break;
                case 86: //sorcerer
                    playerManager.shotSpeedCoefficient *= 2f;
                    spawner.MeteorCoefficient /= 2f;
                    break;
                case 87: //speed_3
                    third = true;
                    break;
                case 88: //speed_booster
                    playerManager.criticalProb += 0.1f;
                    playerManager.shotSpeedCoefficient *= 1.25f;
                    break;
                case 89: //stability
                    playerManager.criticalCoefficient += 0.01f * (player.KillNum > 60 ? 60 : player.KillNum);
                    break;
                case 90: //stunning_strike
                    playerManager.fatalProb += 0.15f;
                    spawner.MeteorCoefficient += 0.25f;
                    break;
                case 91: //swarm
                    currentAbility.SkillCooltimeMax = 5f;
                    break;
                case 92: //taunt
                    playerManager.fatalProb += 0.5f;
                    hp.Lethal = true;
                    break;
                case 93: //tremor
                    spawner.MeteorCoefficient += 0.25f;
                    exp.ExpMax -= 10;
                    break;
                case 94: //ultimatum
                    spawner.MeteorCoefficient += 0.25f;
                    playerManager.damageCoefficient += 0.25f;
                    break;
                case 95: //unleash
                    spawner.SpeedCoefficient *= 1.1f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.8f);
                    break;
                case 96: //unrelenting
                    currentAbility.SkillCooltimeMax = 4f;
                    break;
                case 97: //unwavering
                    playerManager.damage += 0.5f;
                    playerManager.criticalCoefficient += 0.1f;
                    break;
                case 98: //void
                    currentAbility.SkillCooltimeMax = 5f;
                    break;
                case 99: //vulnerability
                    hp.MaxHP = Mathf.CeilToInt(hp.MaxHP * 0.65f);
                    hp.CurrentHP = Mathf.CeilToInt(hp.CurrentHP * 0.65f);
                    hp.SetHealthBar();
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.65f);
                    break;
                case 100: //warrior
                    playerManager.damageCoefficient += 0.2f;
                    hp.SetHealthBar();
                    break;
                case 101: //volcano
                    spawner.MakeMeteor = true;
                    playerManager.damageCoefficient += 0.3f;
                    break;
                case 102: //berkano
                    playerManager.GetWing(1);
                    playerManager.damage += 0.1f * GameObject.FindGameObjectsWithTag("Wing").Length;
                    break;
                case 103: //ehwaz
                    playerManager.damage += 1f;
                    timer.RoundNum += 1;
                    break;
                case 104: //ansuz
                    playerManager.fixDamage += 0.5f;
                    break;
                case 105: //hagalaz
                    spawner.Disabled = true;
                    break;
                case 106: //jera
                    playerManager.jera = true;
                    break;
                case 107: //dagaz
                    playerManager.dagaz = true;
                    break;
                case 108: //algiz
                    hp.GetBarrier(5);
                    break;
                case 109: //perthro
                    playerManager.refresh += 10;
                    break;
                case 110: //shard
                    playerManager.damage += 0.1f;
                    playerManager.criticalProb += 0.03f;
                    playerManager.criticalCoefficient += 0.03f;
                    hp.GetBarrier(1);
                    break;
                case 111: //spirit
                    playerManager.damage += 0.01f * (player.KillNum > 60 ? 60 : player.KillNum);
                    break;
                case 112: //cancer
                    player.ChangeSkillCool(player.SkillCooltimeMax - 0.05f);
                    break;
                case 113: //leo
                    hp.MaxHP *= 2f;
                    hp.CurrentHP = hp.MaxHP;
                    hp.SetHealthBar();
                    break;
                case 114: //taurus
                    currentAbility.SkillCooltimeMax = 10f;
                    break;
                case 115: //aries
                    playerManager.GetWing(1);
                    hp.GetBarrier(1);
                    break;
                case 116: //virgo
                    hp.Virgo = true;
                    break;
                case 117: //libra
                    var temp = Enemy.ItemProb;
                    Enemy.ItemProb = playerManager.criticalProb;
                    playerManager.criticalProb = temp;
                    break;
                case 118: //sagittarius
                    spawner.AddHP -= 2f;
                    break;
                case 119: //capricon
                    hp.GetBarrier(1);
                    hp.Capricon = true;
                    break;
                case 120: //aquaris
                    playerManager.criticalProb += 0.05f;
                    playerManager.aquaris = true;
                    break;
                case 121: //pisces
                    playerManager.fixDamage += 0.1f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.9f);
                    break;
                case 122: //scorpio
                    playerManager.fatalProb += 0.15f;
                    player.ChangeSkillCool(player.SkillCooltimeMax + 0.05f);
                    break;
                case 123: //gemini
                    spawner.SpawnSmall = true;
                    timer.Addition += 3;
                    break;
                case 124: //ophiuchus
                    spawner.SpawnRandom = true;
                    playerManager.damage += 1f;
                    break;
                case 125: //pluto
                    spawner.MeteorCoefficient += 0.25f;
                    player.ChangeSkillCool(player.SkillCooltimeMax * 0.75f);
                    break;
                case 126: //overload
                    playerManager.fixDamage *= 2f;
                    break;
                case 127: //horn
                    playerManager.GetWing(1);
                    WingBullet.Damage += 0.5f;
                    break;
                case 128: //blank
                    playerManager.fatalProb += 0.05f;
                    Fireball.FatalDamage += 0.5f;
                    break;
                case 129: //contract
                    playerManager.GetWing(1);
                    WingBullet.Speed *= 1.5f;
                    playerManager.shotSpeedCoefficient *= 1.5f;
                    break;
                case 130: //repair
                    playerManager.repair = true;
                    break;
                case 131: //heat
                    playerManager.GetWing(1);
                    foreach (var w in GameManager.Instance.PlayerManager.Wings) {
                        w.ChangeSkillCool(Wing.SkillCooltimeMax * 0.8f);
                    }
                    break;
                case 132: //lower
                    playerManager.GetWing(1);
                    Wing.Freezing = true;
                    break;
                case 133: //overclock
                    playerManager.GetWing(1);
                    WingBullet.Udo = true;
                    break;
                case 134: //tick
                    Enemy.DamageCoefficient += 0.1f;
                    break;
                case 135: //supply
                    currentAbility.SkillCooltimeMax = 10f;
                    break;
            }
        }

        GameObject GetChildWithName(GameObject obj, string name)
        {
            var childTrans = obj.transform.Find(name);
            return childTrans != null ? childTrans.gameObject : null;
        }
    }
}
