using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour {
    public GameObject GenericSkillTile;
    public float SkillTileWidth;
    public float SkillTileHeight;
    public int NumTilesPerRow;
    public float SkillTileStartingOffsetX;
    public float SkillTileStartingOffsetY;

    private Queue<SkillTileInfo> toRender;
    private bool[] slots = new bool[50]; // what does this default to?

    private float rowWidth;

    void Awake ()
    {
        rowWidth = NumTilesPerRow * SkillTileWidth;
        toRender = new Queue<SkillTileInfo>();
    }

    void OnEnable ()
    {
        while(toRender.Count != 0)
        {
            SkillTileInfo info = toRender.Dequeue();
            int indexOfNewTile = AddToInventorySlot();
            Vector2 pos = GetLocalPositionFromSlotIndex(indexOfNewTile);
            InstantiateNewTile(info, pos);
        }
    }

    public void EnqueueNewSkill(SkillTileInfo info)
    {
        toRender.Enqueue(info);
    }

    private Vector2 GetLocalPositionFromSlotIndex(int index)
    {
        float x = (index * SkillTileWidth) % rowWidth;
        float y = index / NumTilesPerRow;
        return new Vector2(x + SkillTileStartingOffsetX, y + SkillTileStartingOffsetY);
    }

    private int GetSlotIndexFromLocalPosition(Vector2 pos)
    {
        return -1;
    }

    private void InstantiateNewTile(SkillTileInfo info, Vector2 pos)
    {
        GameObject newTile = Instantiate(GenericSkillTile) as GameObject;
        newTile.transform.parent = transform; // Make sure this is on the panel.
        newTile.transform.localPosition = pos;
        // TODO: set the image and weapon.
    }

    private int AddToInventorySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == false)
            {
                slots[i] = true;
                return i;
            }
        }

        print("something went horribly wrong and your inventory filled up.");
        return -1;
    }

    public struct SkillTileInfo
    {
        public Image Image;
        public Weapon Weapon;

        public SkillTileInfo(Image image, Weapon weapon)
        {
            Image = image;
            Weapon = weapon;
        }
    }
}
