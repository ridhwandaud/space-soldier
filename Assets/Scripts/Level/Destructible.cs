using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {
    public int health;
    [SerializeField]
    private bool invincible = false;

	public void InflictDamage(int damagePoints)
    {
        if(invincible)
        {
            return;
        }

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

    public void SetInvincibility(bool invincible)
    {
        this.invincible = invincible;
    }
}
