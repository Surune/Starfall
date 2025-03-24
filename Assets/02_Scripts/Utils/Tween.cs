using UnityEngine;
using DG.Tweening;

namespace Starfall.Utils
{
    public class Tween : MonoBehaviour
    {
        [SerializeField] GameObject _obj;
        [SerializeField] float _tweenTime;
        
        public void DoTween()
        {
            transform.rotation = Quaternion.Euler(-450f, 0f, 0f);
            Time.timeScale = 1;
            transform.DORotate(new Vector3(0f, 0f, 0f), _tweenTime).SetEase(Ease.Linear);
        }
    }
}
