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

        // My original idea of doing the energy management here in a generic way is just not playing nicely at all with the
        // charge gun because of its unusual energy consumption patterns. The interface is basically broken now since I had to hack
        // the energy requirement functions and add the logic into the click handler. Lesson learned: too much abstraction = very
        // inflexible. Better to err on the side of too little abstraction and refactor later once I have a better understanding of
        // my use cases and the variations. Also, TODO: Fix the shit.
        if (Input.GetMouseButton(1) && Player.PlayerEnergy.HasEnoughEnergy(rightGun.GetEnergyRequirement()))
        {
            Player.PlayerEnergy.energy -= rightGun.Click(transform);
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
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
