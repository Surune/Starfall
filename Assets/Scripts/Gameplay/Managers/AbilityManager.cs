using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using Audio;
using Data.Abilities;
using Data.Synergies;
using Gameplay.Abilities;
using Gameplay.Entities;
using Gameplay.Spawning;

namespace Gameplay.Managers
{
    public class AbilityManager : MonoBehaviour
    {
        Player player => GameManager.Instance.Player;
        HPManager hp => GameManager.Instance.HPManager;
        Spawner spawner => GameManager.Instance.Spawner;
        SoundManager sound => GameManager.Instance.SoundManager;
        PlayerManager playerManager => GameManager.Instance.PlayerManager;

        public List<Ability> Abilities;
        public int AbilityCount => abilityDatabase.Abilities.Count;

        [SerializeField] private Ability abilityPrefab;
        [SerializeField] private GameObject content;
        [SerializeField] private AbilityDatabase abilityDatabase;
        [SerializeField] private SynergyDatabase synergyDatabase;

        private Dictionary<AbilitySynergy, SynergyData> synergyDataByType;
        private Dictionary<AbilitySynergy, int> synergyCountByType;

        private void Awake()
        {
            Abilities = new List<Ability>();
            synergyDataByType = new Dictionary<AbilitySynergy, SynergyData>();
            synergyCountByType = new Dictionary<AbilitySynergy, int>();
            foreach (var synergy in synergyDatabase.Synergies)
            {
                synergyDataByType.Add(synergy.Type, synergy);
                synergyCountByType.Add(synergy.Type, 0);
            }
        }

        public SynergyData GetSynergyData(AbilitySynergy synergyType)
        {
            return synergyDataByType[synergyType];
        }

        public int GetSynergyCount(AbilitySynergy synergyType)
        {
            return synergyCountByType[synergyType];
        }

        public AbilityData GetRandomAbility()
        {
            return abilityDatabase.Abilities[Random.Range(0, AbilityCount)];
        }

        private void GetSynergy(AbilitySynergy synergyType)
        {
            if (synergyType == AbilitySynergy.None)
            {
                return;
            }

            var synergy = GetSynergyData(synergyType);
            synergyCountByType[synergyType]++;
            if (synergyCountByType[synergyType] != synergy.Requirement)
            {
                return;
            }

            sound.PlaySFX(SoundKey.Synergy);
            ApplyModifiers(synergy.Modifiers);
            switch (synergy.Effect)
            {
                case SynergyEffect.DisableSpawner:
                    spawner.Disabled = true;
                    break;
                case SynergyEffect.EnableStatikk:
                    playerManager.statikk = true;
                    break;
                case SynergyEffect.SetCriticalProbabilityToOne:
                    playerManager.criticalProb = playerManager.criticalProb < 1f ? 1f : playerManager.criticalProb;
                    break;
            }
        }

        public void Choiced(AbilityData ability)
        {
            GameManager.Instance.SelectedAbilities.Add(ability);
            var chosenAbility = Instantiate(abilityPrefab, content.transform);
            chosenAbility.Configure(ability, ability.Synergy1 == AbilitySynergy.None ? Color.clear : GetSynergyData(ability.Synergy1).Color);
            Abilities.Add(chosenAbility);

            GetSynergy(ability.Synergy1);
            GetSynergy(ability.Synergy2);
            if (ability.Type == AbilityType.Passive)
            {
                ApplyModifiers(ability.Modifiers);
            }
        }

        public void ApplyModifiers(List<AbilityModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                ApplyModifier(modifier);
            }
        }

        private void ApplyModifier(AbilityModifier modifier)
        {
            switch (modifier.Stat)
            {
                case AbilityStat.Damage:
                    Apply(ref playerManager.damage, modifier);
                    break;
                case AbilityStat.DamageCoefficient:
                    Apply(ref playerManager.damageCoefficient, modifier);
                    break;
                case AbilityStat.CriticalProbability:
                    Apply(ref playerManager.criticalProb, modifier);
                    break;
                case AbilityStat.CriticalCoefficient:
                    Apply(ref playerManager.criticalCoefficient, modifier);
                    break;
                case AbilityStat.FatalProbability:
                    Apply(ref playerManager.fatalProb, modifier);
                    break;
                case AbilityStat.ShotSpeedCoefficient:
                    Apply(ref playerManager.shotSpeedCoefficient, modifier);
                    break;
                case AbilityStat.SkillCooldown:
                    var cooldown = player.SkillCooltimeMax;
                    Apply(ref cooldown, modifier);
                    player.ChangeSkillCool(cooldown);
                    break;
                case AbilityStat.MaxHealth:
                    Apply(ref hp.MaxHP, modifier);
                    hp.SetHealthBar();
                    break;
                case AbilityStat.CurrentHealth:
                    if (modifier.Operation == AbilityModifierOperation.RestoreToMaximum)
                    {
                        hp.CurrentHP = hp.MaxHP;
                    }
                    else
                    {
                        Apply(ref hp.CurrentHP, modifier);
                    }
                    hp.SetHealthBar();
                    break;
                case AbilityStat.Barrier:
                    hp.GetBarrier(Mathf.RoundToInt(modifier.Value));
                    break;
                case AbilityStat.EnemyDamageCoefficient:
                    Apply(ref Enemy.DamageCoefficient, modifier);
                    break;
                case AbilityStat.EnemyItemProbability:
                    Apply(ref Enemy.ItemProb, modifier);
                    break;
                case AbilityStat.EnemySpeedCoefficient:
                    Apply(ref spawner.SpeedCoefficient, modifier);
                    break;
                case AbilityStat.Refresh:
                    playerManager.refresh += Mathf.RoundToInt(modifier.Value);
                    break;
                case AbilityStat.CoinCoefficient:
                    GameManager.Instance.CoinCoefficient += modifier.Value;
                    break;
                case AbilityStat.BulletCount:
                    Apply(ref player.BulletCount, modifier);
                    break;
                case AbilityStat.WingCount:
                    playerManager.GetWing(Mathf.RoundToInt(modifier.Value));
                    break;
            }
        }

        private static void Apply(ref float stat, AbilityModifier modifier)
        {
            if (modifier.Operation == AbilityModifierOperation.Add)
            {
                stat += modifier.Value;
                return;
            }

            stat *= modifier.Value;
        }

        private static void Apply(ref int stat, AbilityModifier modifier)
        {
            if (modifier.Operation == AbilityModifierOperation.Add)
            {
                stat += Mathf.RoundToInt(modifier.Value);
                return;
            }

            stat = Mathf.RoundToInt(stat * modifier.Value);
        }
    }
}
