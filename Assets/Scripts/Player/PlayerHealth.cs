using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int healthPoints = 30;
    public Slider healthSlider;

	void OnTriggerEnter2D (Collider2D other) {
        if (other.tag.Equals("Fireball"))
        {
            healthPoints--;
            other.GetComponent<ProjectileDestroy>().Destroy();
        }

        // Only do this if the player takes damage?
        healthSlider.value = healthPoints;

        if (healthPoints == 0)
        {
            gameObject.SetActive(false);
        }
	}
}
