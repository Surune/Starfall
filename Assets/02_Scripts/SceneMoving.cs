using UnityEngine;

namespace Starfall.Manager {
    public class SceneMovementManager : MonoBehaviour {
        [SerializeField] private AudioSource musicPlayer;
        [SerializeField] private AudioClip sfxMenu;

        public void goto_MainGame() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("maingame");
            musicPlayer.PlayOneShot(sfxMenu);
        }

        public void goto_Start() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("start");
            musicPlayer.PlayOneShot(sfxMenu);
        }

        public void goto_Ready() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ready");
            musicPlayer.PlayOneShot(sfxMenu);
        }

        public void goto_Options() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("options");
            musicPlayer.PlayOneShot(sfxMenu);
        }

        public void goto_Howtoplay() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("howtoplay");
            musicPlayer.PlayOneShot(sfxMenu);
        }
    }
}