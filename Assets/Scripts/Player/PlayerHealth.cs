using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private GameObject gameOverUI;

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
            ShowGameOverScreen();
        }

        Camera.main.GetComponent<SporeEffect>().ProcessSpore() ;
    }

    void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
    }
}
