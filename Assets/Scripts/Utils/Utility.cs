using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Starfall.Utils
{
    public static class Utility
    {
        public static Transform FindClosestTransform(List<Transform> t_list, Vector3 pos)
        {
            Transform tMin = null;
            var minDist = Mathf.Infinity;
            foreach (var t in t_list)
            {
                var dist = Vector3.Distance(t.position, pos);
                if (!(dist < minDist))
                {
                    continue;
                }
                tMin = t;
                minDist = dist;
            }
            return tMin;
        }

        public static List<Transform> GetAllChildren(this Transform t)
        {
            return t.Cast<Transform>().Where(tranform => tranform.gameObject.activeSelf).ToList();
        }
    }
}