using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerWeaponControl : MonoBehaviour {

    public int energyPoints;
    public float firingDelay = .1f;
    public float bulletSpeed = 50;
    public Slider energySlider;

    private float nextFiringTime;

    private Weapon leftGun;
    private Weapon rightGun;
    private List<Weapon> weapons;

    private int currentRightWeaponIndex = 1;

	void Awake () {
        nextFiringTime = 0;

        weapons = new List<Weapon> {
            GetComponentInChildren<ChargeGun>(),
            GetComponentInChildren<MachineGun>(),
            GetComponentInChildren<MultiShot>(),
            GetComponentInChildren<EnergyGun>()
        };

        leftGun = weapons[0];
        rightGun = weapons[1];
	}
	
	void Update () {
        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0))
        {
            leftGun.Click(transform);
        }

        if (Input.GetMouseButtonUp(0))
        {
            leftGun.Release(transform);
        }

        if (Input.GetMouseButton(1))
        {
            if (energyPoints >= rightGun.GetEnergyRequirement())
            {
                energyPoints -= rightGun.Click(transform);
            }
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
            ToggleRightWeapon();
        }

        if (Input.GetMouseButtonUp(1))
        {
            energyPoints -= rightGun.Release(transform);
        }

        energySlider.value = energyPoints;
	}

    public void AddWeapon(Weapon newWeapon)
    {
        weapons.Add(newWeapon);
    }

    public void ToggleRightWeapon()
    {
        do
        {
            currentRightWeaponIndex = currentRightWeaponIndex == weapons.Count - 1 ? 0 : ++currentRightWeaponIndex;
        } while (weapons[currentRightWeaponIndex] == leftGun);

        rightGun = weapons[currentRightWeaponIndex];
    }
}
