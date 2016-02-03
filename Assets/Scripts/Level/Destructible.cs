using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {
    public int health;

	public void InflictDamage(int damagePoints)
    {
        health -= damagePoints;
        if (health <= 0)
        {
            Destroy(gameObject);
            if (GameState.TutorialMode)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.ItemDestroyed);
            }
        }
    }
}
