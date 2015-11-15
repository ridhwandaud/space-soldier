using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour {
    public GameObject GenericInventoryTile;
    public float InventoryTileWidth;
    public float InventoryTileHeight;
    public int NumTilesPerRow;
    public float InventoryTileStartingOffsetX;
    public float InventoryTileStartingOffsetY;
    public float PaddingBetweenTiles;
    public int MaxTiles;

    private Queue<InventoryTileInfo> toRender;
    private bool[] slots;
    private float cellWidth;
    private float cellHeight;

    private float rowWidth;

    void Update ()
    {
        if (Input.GetButtonDown("Jump"))
        {
            EnqueueNewSkill(new InventoryTileInfo());
        }

        if (Input.GetButtonDown("Submit"))
        {
            OnEnable();
        }
    }

    void Awake ()
    {
        // temporary code to pause game for testing
        Time.timeScale = 0;

        slots = new bool[MaxTiles];
        cellWidth = InventoryTileWidth + PaddingBetweenTiles;
        cellHeight = InventoryTileHeight + PaddingBetweenTiles;
        rowWidth = NumTilesPerRow * cellWidth;
        toRender = new Queue<InventoryTileInfo>();
    }

    void OnEnable ()
    {
        while(toRender.Count != 0)
        {
            InventoryTileInfo info = toRender.Dequeue();
            int indexOfNewTile = AddToInventorySlot();
            Vector2 pos = GetLocalPositionFromSlotIndex(indexOfNewTile);
            InstantiateNewTile(info, pos);
        }
    }

    public void EnqueueNewSkill(InventoryTileInfo info)
    {
        toRender.Enqueue(info);
    }

    private Vector2 GetLocalPositionFromSlotIndex(int index)
    {
        float x = (index * cellWidth) % rowWidth;
        float y = index / NumTilesPerRow * cellHeight;
        return new Vector2(x + InventoryTileStartingOffsetX, InventoryTileStartingOffsetY - y);
    }

    private int GetSlotIndexFromLocalPosition(Vector2 pos)
    {
        return -1;
    }

    private void InstantiateNewTile(InventoryTileInfo info, Vector2 pos)
    {
        GameObject newTile = Instantiate(GenericInventoryTile) as GameObject;
        newTile.transform.SetParent(transform);
        newTile.transform.localPosition = pos;
        newTile.GetComponent<InventoryTile>().Init();
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

    public struct InventoryTileInfo
    {
        public Image Image;
        public Weapon Weapon;

        public InventoryTileInfo(Image image, Weapon weapon)
        {
            Image = image;
            Weapon = weapon;
        }
    }
}
