using System.Collections.Generic;
using Starfall.Constants;
using UnityEngine;

namespace Starfall.Manager {
    public class PoolManager : MonoBehaviour {
        public GameObject[] prefabs;
        public Transform[] contents;
        List<GameObject>[] pools;

        private const int maxItem = 100;

        void Awake() {
            pools = new List<GameObject>[prefabs.Length];

            for (int idx = 0; idx < pools.Length; idx++) {
                pools[idx] = new List<GameObject>();
            }
        }

        public GameObject Get(PoolNumber num) {
            GameObject select = null;
            var index = (int)num;

            foreach (GameObject item in pools[index]) {
                if(!item.activeSelf) {
                    select = item;
                    select.SetActive(true);
                    break;
                }
            }
            if(!select) {
                if (index == 1 || pools[index].Count < maxItem){
                    // 새롭게 생성하여 select에 할당
                    select = Instantiate(prefabs[index], contents[index]);
                    pools[index].Add(select);
                }
                else {
                    select = pools[index][0];
                    pools[index].RemoveAt(0);
                    pools[index].Add(select);
                }
            }

            return select;
        }
    }
}