using UnityEngine;

namespace Starfall.Manager
{
    public class SceneMoving : MonoBehaviour
    {
        [SerializeField] AudioSource _musicPlayer;
        [SerializeField] AudioClip _sfxMenu;

        public void Goto_MainGame()
        {
            GotoScene("maingame");
        }

        public void Goto_Start()
        {
            GotoScene("start");
        }

        public void Goto_Ready()
        {
            GotoScene("ready");
        }

        public void Goto_Options()
        {
            GotoScene("options");
        }

        public void Goto_Howtoplay()
        {
            GotoScene("howtoplay");
        }

        void GotoScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            _musicPlayer.PlayOneShot(_sfxMenu);
        }
    }
}
