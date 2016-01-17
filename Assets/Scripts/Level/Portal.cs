using UnityEngine;

public class Portal : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameState.LevelIndex++;
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
