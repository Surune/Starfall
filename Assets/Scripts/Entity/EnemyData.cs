using System.Collections.Generic;
using Starfall.Constants;
using UnityEngine;

namespace Starfall.Entity
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Starfall/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public EnemyType Type;
        public float BaseHP;
        public float StageHP;
        public float Speed;
        public List<Sprite> Sprites;
    }
}
