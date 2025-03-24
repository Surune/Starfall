using UnityEngine;

namespace Starfall.Utils
{
    public class Openurl
    {
        [SerializeField] readonly string _url;

        public void OpenLink()
        {
            Application.OpenURL(_url);
        }
    }
}
