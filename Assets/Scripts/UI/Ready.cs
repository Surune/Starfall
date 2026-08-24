using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Networking;

namespace UI
{
    public class Ready : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button startButton;

        private void Awake()
        {
            backButton.onClick.AddListener(SceneMoving.Goto_Lobby);
            startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            if (NetworkServer.active)
            {
                NetworkSessionManager.Instance.StartMatch();
                return;
            }

            if (!NetworkClient.isConnected)
            {
                SceneMoving.Goto_MainGame();
            }
        }
    }
}
