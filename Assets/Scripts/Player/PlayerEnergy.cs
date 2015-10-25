using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour {
    public float energyMax;
    public float energy;
    public float energyRechargeRate;
    public Slider energySlider;

    private bool rechargePaused = false;

    void OnLevelWasLoaded ()
    {
        energy = energyMax;
    }

    void Awake ()
    {
        energySlider.maxValue = energyMax;
        energySlider.value = energyMax;

        energy = energyMax;
    }

	void Update () {
        if (!rechargePaused)
        {
            energy = Mathf.Min(energyMax, energy + energyRechargeRate * Time.deltaTime);
        }

        energySlider.value = energy;
	}

    public bool HasEnoughEnergy(float energyRequirement)
    {
        return energy >= energyRequirement;
    }

    public void PauseRecharge()
    {
        rechargePaused = true;
    }

    public void UnpauseRecharge()
    {
        rechargePaused = false;
    }
}
