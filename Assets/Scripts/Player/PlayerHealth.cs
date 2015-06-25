using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int healthPoints = 3;

	void OnTriggerEnter2D (Collider2D other) {
        if (other.tag.Equals("Fireball"))
        {
            healthPoints--;
            other.GetComponent<ProjectileDestroy>().Destroy();
        }

        if (healthPoints == 0)
        {
            gameObject.SetActive(false);
        }
	}
}
