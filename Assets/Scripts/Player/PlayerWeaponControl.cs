using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerWeaponControl : MonoBehaviour {

    public float firingDelay;
    public float bulletSpeed;
    public Slider energySlider;

    private Weapon leftGun;
    private Weapon rightGun;
    private List<Weapon> weapons;

    private int currentRightWeaponIndex = 1;

	void Awake () {
        weapons = new List<Weapon> {
            GetComponentInChildren<Pistol>(),
            GetComponentInChildren<MachineGun>(),
            GetComponentInChildren<ChargeGun>(),
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
            if (Player.PlayerEnergy.HasEnoughEnergy(rightGun.GetEnergyRequirement()))
            {
                Player.PlayerEnergy.energy -= rightGun.Click(transform);
            }
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
            //Player.PlayerEnergy.energy -= rightGun.Release(transform);
            ToggleRightWeapon();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Player.PlayerEnergy.energy -= rightGun.Release(transform);
        }
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
