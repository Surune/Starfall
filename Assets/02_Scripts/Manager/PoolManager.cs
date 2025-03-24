using System.Collections.Generic;
using System.Linq;
using Starfall.Constants;
using UnityEngine;

namespace Starfall.Manager
{
    public class PoolManager : MonoBehaviour
    {
        public GameObject[] Prefabs;
        public Transform[] Contents;
        List<GameObject>[] pools;

        const int MaxItem = 100;

        void Start()
        {
            pools = new List<GameObject>[Prefabs.Length];

            for (int idx = 0; idx < pools.Length; idx++)
            {
                pools[idx] = new List<GameObject>();
            }
        }

        public GameObject Get(PoolNumber num)
        {
            GameObject select = null;
            var index = (int)num;

            foreach (var item in pools[index].Where(item => !item.activeSelf))
            {
                select = item;
                select.SetActive(true);
                break;
            }

            if (select)
                return select;

            if (index == 1 || pools[index].Count < MaxItem)
            {
                // 새롭게 생성하여 select에 할당
                select = Instantiate(Prefabs[index], Contents[index]);
                pools[index].Add(select);
            }
            else {
                select = pools[index][0];
                pools[index].RemoveAt(0);
                pools[index].Add(select);
            }

            return select;
        }
    }
}
