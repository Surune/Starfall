using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using Audio;
using Data.Abilities;
using Data.Synergies;
using Gameplay.Abilities;

namespace Gameplay.Managers
{
    public class AbilityManager : MonoBehaviour
    {
        SoundManager sound => GameManager.Instance.SoundManager;
        PlayerManager playerManager => GameManager.Instance.PlayerManager;

        [SerializeField] private Ability abilityPrefab;
        [SerializeField] private GameObject content;
        [SerializeField] private AbilityDatabase abilityDatabase;
        [SerializeField] private SynergyDatabase synergyDatabase;

        private AbilityModifierApplier modifierApplier;
        private Dictionary<AbilitySynergy, SynergyData> synergyDataByType;
        private Dictionary<AbilitySynergy, int> synergyCountByType;
        private int AbilityCount => abilityDatabase.Abilities.Count;

        private void Awake()
        {
            modifierApplier = new AbilityModifierApplier(GameManager.Instance.Player, GameManager.Instance.HPManager, GameManager.Instance.Spawner, GameManager.Instance.PlayerManager, GameManager.Instance);
            
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
                modifierApplier.Apply(modifier);
            }
        }
    }
}
