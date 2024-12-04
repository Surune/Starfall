namespace Starfall.Manager {
    public enum GameState {
        Gameplay,
        Paused
    }

    public class GameStateManager {
        public static GameStateManager Instance {
            get {
                if (instance == null)
                    instance = new GameStateManager();

                return instance;
            }
        }

        public bool IsPlaying => currentGameState == GameState.Gameplay;

        private GameState currentGameState;
        public delegate void GameStateChangeHandler(GameState newGameState);
        public event GameStateChangeHandler OnGameStateChanged;

        private static GameStateManager instance;

        public void SetState(GameState newGameState) {
            if (newGameState == currentGameState)
                return;

            currentGameState = newGameState;
            OnGameStateChanged?.Invoke(newGameState);
        }
    }
}