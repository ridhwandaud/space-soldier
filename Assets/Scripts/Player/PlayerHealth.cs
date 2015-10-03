using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int healthPoints;
    public Slider healthSlider;

    void Awake()
    {
        healthSlider.maxValue = healthPoints;
        healthSlider.value = healthPoints;
    }

    public void InflictDamage(int damage)
    {
        healthPoints -= damage;
        healthSlider.value = healthPoints;

        if (healthPoints <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
