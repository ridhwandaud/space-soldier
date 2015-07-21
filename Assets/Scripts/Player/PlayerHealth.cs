using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int healthPoints = 15;
    public Slider healthSlider;

    public void InflictDamage(int damage)
    {
        healthPoints -= damage;
        healthSlider.value = healthPoints;

        if (healthPoints == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
