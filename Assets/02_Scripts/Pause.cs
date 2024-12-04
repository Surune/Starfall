using UnityEngine;
using Starfall.Manager;

public class Pause : MonoBehaviour {
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject ability;
    public Camera myCamera;

    public void PauseOnClick() {
        if (GameStateManager.Instance.IsPlaying) {
            GameStateManager.Instance.SetState(GameState.Paused);
            text.SetActive(true);
            myCamera.cullingMask = ~( (1 << 3) | (1 << 6) );
        }
        else {
            GameStateManager.Instance.SetState(GameState.Gameplay);
            text.SetActive(false);
            myCamera.cullingMask = -1;
        }
    }
}
