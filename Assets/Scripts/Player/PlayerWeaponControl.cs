using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponControl : MonoBehaviour {

    public float firingDelay;
    public float bulletSpeed;
    public Slider energySlider;
    public Weapon[] leftWeapons;
    public Weapon[] rightWeapons;

    private Weapon leftGun;
    private Weapon rightGun;

    private int currentLeftWeaponIndex = 0;
    private int currentRightWeaponIndex = 0;

	void Awake () {
        leftGun = leftWeapons[0];
        rightGun = rightWeapons[0];
	}
	
	void Update () {
        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0) && leftGun != null)
        {
            leftGun.Click(transform);
        }

        if (Input.GetMouseButtonUp(0) && leftGun != null)
        {
            leftGun.Release(transform);
        }

        // My original idea of doing the energy management here in a generic way is just not playing nicely at all with the
        // charge gun because of its unusual energy consumption patterns. The interface is basically broken now since I had to hack
        // the energy requirement functions and add the logic into the click handler. Lesson learned: too much abstraction = very
        // inflexible. Better to err on the side of too little abstraction and refactor later once I have a better understanding of
        // my use cases and the variations. Also, TODO: Fix the shit.
        if (Input.GetMouseButton(1) && rightGun != null && Player.PlayerEnergy.HasEnoughEnergy(rightGun.GetEnergyRequirement()))
        {
            print("FIRING YO");
            Player.PlayerEnergy.energy -= rightGun.Click(transform);
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
            ToggleRightWeapon();
        }

        if (Input.GetMouseButtonUp(1) && rightGun != null)
        {
            Player.PlayerEnergy.energy -= rightGun.Release(transform);
        }
	}

    // Will be removed once inventory screen is created.
    public void AddWeapon(Weapon newWeapon, WeaponSide weaponSide)
    {
        Weapon[] holster = weaponSide == WeaponSide.Left ? leftWeapons : rightWeapons;

        for (int i = 0; i < holster.Length; i++)
        {
            if (holster[i] == null)
            {
                holster[i] = newWeapon;
                if (rightGun == null)
                {
                    rightGun = newWeapon;
                }
                return;
            }
        }

        print("holster is full. failed to add weapon.");
    }

    // For use by inventory.
    public void SetWeapon(Weapon newWeapon, WeaponSide weaponSide, int slot)
    {
        Weapon[] holster = weaponSide == WeaponSide.Left ? leftWeapons : rightWeapons;
        holster[slot] = newWeapon;
    }

    void ToggleRightWeapon()
    {
        do
        {
            currentRightWeaponIndex = currentRightWeaponIndex == rightWeapons.Length - 1 ? 0 : ++currentRightWeaponIndex;
        } while (rightWeapons[currentRightWeaponIndex] == null);

        rightGun = rightWeapons[currentRightWeaponIndex];
    }

    // It's actually cleaner to separate these functions then to try to merge them into one.
    void ToggleLeftWeapon ()
    {
        do
        {
            currentLeftWeaponIndex = currentLeftWeaponIndex == leftWeapons.Length - 1 ? 0 : ++currentLeftWeaponIndex;
        } while (leftWeapons[currentLeftWeaponIndex] == null);

        leftGun = leftWeapons[currentLeftWeaponIndex];
    }

    public enum WeaponSide { Left, Right };
}
