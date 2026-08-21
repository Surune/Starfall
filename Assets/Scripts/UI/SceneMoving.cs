using UnityEngine;
using Audio;
using Gameplay.Managers;

namespace UI
{
    public class SceneMoving : MonoBehaviour
    {
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

        private void GotoScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            GameManager.Instance.SoundManager.PlaySFX(SoundKey.MoveScene);
        }
    }
}
