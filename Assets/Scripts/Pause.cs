using UnityEngine;
using Starfall.Manager;
using Starfall.Constants;
using UnityEngine.Serialization;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject text;
    [SerializeField] GameObject content;
    [SerializeField] GameObject ability;
    public Camera MyCamera;

    public void PauseOnClick()
    {
        if (GameStateManager.Instance.IsPlaying)
        {
            GameStateManager.Instance.SetState(GameState.Paused);
            text.SetActive(true);
            MyCamera.cullingMask = ~( (1 << 3) | (1 << 6) );
        }
        else
        {
            GameStateManager.Instance.SetState(GameState.Gameplay);
            text.SetActive(false);
            MyCamera.cullingMask = -1;
        }
    }
}
