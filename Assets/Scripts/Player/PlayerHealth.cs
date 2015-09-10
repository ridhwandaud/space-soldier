using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int healthPoints;
    public Slider healthSlider;

    void Awake()
    {
        healthSlider.value = healthPoints;
        healthSlider.maxValue = healthPoints;
    }

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
