using UnityEngine;
using DG.Tweening;

namespace Starfall.Utils{
    public class Tween : MonoBehaviour {
        [SerializeField] private GameObject obj;
        [SerializeField] private float tweenTime;
        
        public void tween() {
            transform.rotation = Quaternion.Euler(-450f, 0f, 0f);
            Time.timeScale = 1;
            transform.DORotate(new Vector3(0f, 0f, 0f), tweenTime).SetEase(Ease.Linear);
        }
    }
}