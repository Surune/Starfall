using System.Collections.Generic;
using UnityEngine;

namespace Data.Synergies
{
    [CreateAssetMenu(menuName = "Starfall/Synergy Database", fileName = "SynergyDatabase")]
    public class SynergyDatabase : ScriptableObject
    {
        public List<SynergyData> Synergies;
    }
}
