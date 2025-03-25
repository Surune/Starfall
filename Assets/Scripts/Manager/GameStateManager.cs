using Starfall.Constants;

namespace Starfall.Manager
{
    public class GameStateManager
    {
        public static GameStateManager Instance { get; private set; }

        public bool IsPlaying => currentGameState == GameState.Gameplay;
        GameState currentGameState;

        public GameStateManager()
        {
            Instance = this;
        }

        public void SetState(GameState newGameState)
        {
            currentGameState = newGameState;
        }
    }
}
