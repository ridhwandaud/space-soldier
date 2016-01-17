using UnityEngine;

public class GameState : MonoBehaviour {
    public static int NumEnemiesRemaining = 0;
    public static bool Paused = false;

    // These are for score tracking (to display at the end of the game)
    public static int NumEnemiesKilled = 0;
    public static int LevelIndex = 0;

    public static void ResetGameState()
    {
        NumEnemiesRemaining = 0;
        Paused = false;
        NumEnemiesKilled = 0;
        LevelIndex = 0;
    }
}
