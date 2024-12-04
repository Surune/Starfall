using UnityEngine;

namespace Starfall.Utils {
    public class Openurl {
        [SerializeField] private string url;

        public void openLink() {
            Application.OpenURL(url);
        }
    }
}