using UnityEngine;

public class InventorySlot : MonoBehaviour {
    public bool Occupied {
        get {
            return tile != null;
        }
    }

    private InventoryTile tile;

    public Weapon GetWeaponIfExists()
    {
        return tile ? tile.GetWeapon() : null;
    }

    public void UnsetTile()
    {
        tile = null;
    }

    public void SetTile(InventoryTile tile)
    {
        this.tile = tile;
    }
}