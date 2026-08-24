using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SceneMoving : MonoBehaviour
    {
        public static void Goto_Lobby()
        {
            GotoScene("lobby");
        }

        public static void Goto_Ready()
        {
            GotoScene("ready");
        }

        public static void Goto_MainGame()
        {
            GotoScene("maingame");
        }
        
        private static void GotoScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
