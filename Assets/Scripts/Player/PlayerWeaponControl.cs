using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerWeaponControl : MonoBehaviour {

    [SerializeField]
    private float firingDelay;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private Slider energySlider;
    [SerializeField]
    private Weapon[] leftWeapons;
    [SerializeField]
    private Weapon[] rightWeapons;
    [SerializeField]
    private Weapon defaultStartingWeapon;

    private Weapon leftGun;
    private Weapon rightGun;

    private int currentLeftWeaponIndex = 0;
    private int currentRightWeaponIndex = 0;

    private int numLeftGuns = 1;
    private int numRightGuns = 0;

    private bool leftMouseButtonClicked = false;
    private bool rightMouseButtonClicked = false;

	void Awake () {
        if(!GameState.TutorialMode)
        {
            leftWeapons[0] = leftGun = defaultStartingWeapon;
        }

        StartCoroutine("AddPistolToInventory");
	}

    IEnumerator AddPistolToInventory()
    {
        // Must wait for end of frame so that UI scaling can take place. This ensures that the tile is initialized properly.
        yield return new WaitForEndOfFrame();

        InventoryManager.InventoryTileInfo tileInfo = new InventoryManager.InventoryTileInfo(null, defaultStartingWeapon);

        if (GameState.TutorialMode)
        {
            InventoryManager.Instance.InstantiateTileAtPosition(tileInfo, 6);
        } else
        {
            InventoryManager.Instance.InstantiateNewTile(tileInfo);
        }
    }
	
	void Update () {
        if (GameState.Paused)
        {
            return;
        }

        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0) && leftGun != null)
        {
            leftMouseButtonClicked = true;
            leftGun.Click(transform);
        }

        if (leftMouseButtonClicked && !Input.GetMouseButton(0) && leftGun != null)
        {
            leftMouseButtonClicked = false;
            leftGun.Release(transform);
        }

        // My original idea of doing the energy management here in a generic way is just not playing nicely at all with the
        // charge gun because of its unusual energy consumption patterns. The interface is basically broken now since I had to hack
        // the energy requirement functions and add the logic into the click handler. Lesson learned: too much abstraction = very
        // inflexible. Better to err on the side of too little abstraction and refactor later once I have a better understanding of
        // my use cases and the variations. Also, TODO: Fix the shit.
        if (Input.GetMouseButton(1) && rightGun != null && Player.PlayerEnergy.HasEnoughEnergy(rightGun.GetEnergyRequirement()))
        {
            rightMouseButtonClicked = true;
            Player.PlayerEnergy.energy -= rightGun.Click(transform);
        }

        if (leftMouseButtonClicked && rightMouseButtonClicked && GameState.TutorialMode)
        {
            TutorialEngine.Instance.Trigger(TutorialTrigger.BothGunsFired);
        }

        if (Input.GetButtonDown("ToggleLeftWeapon"))
        {
            ToggleLeftWeapon();
        }

        if (Input.GetButtonDown("ToggleRightWeapon"))
        {
            ToggleRightWeapon();
        }

        if (rightMouseButtonClicked && !Input.GetMouseButton(1) && rightGun != null)
        {
            rightMouseButtonClicked = false;
            Player.PlayerEnergy.energy -= rightGun.Release(transform);
        }
	}

    public bool AddWeaponIfAble(Weapon newWeapon)
    {
        // First, try adding to right side.
        for (int i = 0; i < rightWeapons.Length; i++)
        {
            if (rightWeapons[i] == null)
            {
                rightWeapons[i] = newWeapon;
                IncrementWeaponCount(WeaponSide.Right);
                if (rightGun == null)
                {
                    rightGun = newWeapon;
                }
                return true;
            }
        }

        // If right side is full, try adding to left side.
        for (int i = 0; i < leftWeapons.Length; i++)
        {
            if (leftWeapons[i] == null)
            {
                leftWeapons[i] = newWeapon;
                IncrementWeaponCount(WeaponSide.Left);
                if (leftGun == null)
                {
                    leftGun = newWeapon;
                }
                return true;
            }
        }

        // If both sides are full, return false.
        return false;
    }

    // For use by inventory.
    public void SetWeapon(Weapon newWeapon, WeaponSide weaponSide, int slot)
    {
        Weapon[] weapons = weaponSide == WeaponSide.Left ? leftWeapons : rightWeapons;
        if (weapons[slot] != null)
        {
            print("can't equip. not enough slots.");
            return;
        }

        weapons[slot] = newWeapon;
        IncrementWeaponCount(weaponSide);

        if (weaponSide == WeaponSide.Left && leftGun == null)
        {
            leftGun = newWeapon;
        }
        else if (weaponSide == WeaponSide.Right && rightGun == null)
        {
            rightGun = newWeapon;
        }
    }

    public void UnsetWeapon(WeaponSide weaponSide, int slot)
    {
        Weapon[] weapons = weaponSide == WeaponSide.Left ? leftWeapons : rightWeapons;
        weapons[slot] = null;

        if (weaponSide == WeaponSide.Left)
        {
            ToggleLeftWeapon();
        }
        else
        {
            ToggleRightWeapon();
        }
    }

    private void ToggleWeapon (ref bool mouseButtonClicked, ref int weaponIndex, ref Weapon currentWeapon, Weapon[] weapons, int numGuns)
    {
        int weaponsExamined = 0;
        int originalWeaponIndex = weaponIndex;
        do
        {
            weaponIndex = weaponIndex == weapons.Length - 1 ? 0 : ++weaponIndex;
            weaponsExamined++;
        } while (weapons[weaponIndex] == null && weaponsExamined < weapons.Length);

        if (originalWeaponIndex != weaponIndex && currentWeapon != null && numGuns > 1 && mouseButtonClicked)
        {
            Player.PlayerEnergy.energy -= currentWeapon.Release(transform);
            mouseButtonClicked = false;
        }
        currentWeapon = weapons[weaponIndex];
    }

    void ToggleRightWeapon()
    {
        ToggleWeapon(ref rightMouseButtonClicked, ref currentRightWeaponIndex, ref rightGun, rightWeapons, numRightGuns);
    }

    void ToggleLeftWeapon ()
    {
        ToggleWeapon(ref leftMouseButtonClicked, ref currentLeftWeaponIndex, ref leftGun, leftWeapons, numLeftGuns);
    }

    private void IncrementWeaponCount(WeaponSide side)
    {
        if (side == WeaponSide.Left)
        {
            numLeftGuns++;
        } else
        {
            numRightGuns++;
        }
    }

    private void DecrementWeaponCount (WeaponSide side)
    {
        if (side == WeaponSide.Left)
        {
            numLeftGuns--;
        }
        else
        {
            numRightGuns--;
        }
    }

    public enum WeaponSide { Left, Right };
}
